using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.EventStoreDb.Messaging;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure.Entities;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Threading;

namespace VShop.SharedKernel.EventStoreDb.Subscriptions.Services
{
    public class SubscriptionToAllBackgroundService : ISubscriptionBackgroundService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task _executingTask;
        private readonly object _resubscribeLock = new();

        private readonly ILogger _logger;
        private readonly EventStoreClient _eventStoreClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly SubscriptionConfig _subscriptionConfig;

        public SubscriptionToAllBackgroundService
        (
            ILogger logger,
            EventStoreClient eventStoreClient,
            IServiceProvider serviceProvider,
            SubscriptionConfig subscriptionConfig
        )
        {
            _logger = logger;
            _eventStoreClient = eventStoreClient;
            _serviceProvider = serviceProvider;
            _subscriptionConfig = subscriptionConfig;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _executingTask = SubscribeToAllAsync(_cancellationTokenSource.Token);

            return _executingTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask is null) return;

            // Signal cancellation to the executing method
            _cancellationTokenSource.Cancel();

            // Wait until the issue completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            // Throw if cancellation triggered
            cancellationToken.ThrowIfCancellationRequested();

            _logger.Information("Subscription to all '{SubscriptionId}' stopped", _subscriptionConfig.SubscriptionName);
        }

        private async Task SubscribeToAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Subscription to all '{SubscriptionId}' started", _subscriptionConfig.SubscriptionName);

            using IServiceScope scope = _serviceProvider.CreateScope();
            SubscriptionContext subscriptionContext = scope.ServiceProvider.GetRequiredService<SubscriptionContext>();
            
            Checkpoint checkpoint = await subscriptionContext.Checkpoints
                .FirstOrDefaultAsync(c => c.SubscriptionId == _subscriptionConfig.SubscriptionName, cancellationToken);

            await _eventStoreClient.SubscribeToAllAsync
            (
                GetPosition(checkpoint?.Position),
                HandleEventAsync,
                false,
                HandleDrop,
                _subscriptionConfig.SubscriptionFilterOptions,
                cancellationToken: cancellationToken
            );
        }

        private static Position GetPosition(ulong? checkpoint)
            => checkpoint.HasValue
                ? new Position(checkpoint.Value, checkpoint.Value)
                : Position.Start;

        private async Task HandleEventAsync
        (
            StreamSubscription _,
            ResolvedEvent resolvedEvent,
            CancellationToken cancellationToken
        )
        {
            if (IsMessageWithEmptyData(resolvedEvent) || IsCheckpointMessage(resolvedEvent) || !IsSubscribedToMessage(resolvedEvent)) return;

            try
            {
                IMessageMetadata metadata = resolvedEvent.DeserializeMetadata();
                IMessage message = resolvedEvent.DeserializeData<IMessage>();
                
                // Consuming a scoped service in a background task
                // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-5.0&tabs=visual-studio#consuming-a-scoped-service-in-a-background-task-1
                using IServiceScope scope = _serviceProvider.CreateScope();
                SubscriptionContext subscriptionContext = scope.ServiceProvider.GetRequiredService<SubscriptionContext>();
                
                IExecutionStrategy strategy = subscriptionContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    await using IDbContextTransaction transaction = await subscriptionContext.BeginTransactionAsync(cancellationToken);
                    await _subscriptionConfig.SubscriptionHandler.ProjectAsync(message, metadata, scope, transaction, cancellationToken); // TODO - refactoring needed

                    Checkpoint checkpoint = await subscriptionContext.Checkpoints
                        .FirstOrDefaultAsync(c => c.SubscriptionId == _subscriptionConfig.SubscriptionName, cancellationToken);
                    ulong position = resolvedEvent.Event.Position.CommitPosition;

                    if (checkpoint is not null) checkpoint.Position = position;
                    else
                    {
                        subscriptionContext.Checkpoints.Add(new Checkpoint
                        {
                            SubscriptionId = _subscriptionConfig.SubscriptionName,
                            Position = position
                        });
                    }
                    
                    await subscriptionContext.CommitTransactionAsync(transaction, cancellationToken);
                });
            }
            catch (Exception ex)
            {
                _logger.Error
                (
                    ex,
                    "Error consuming message: {ExceptionMessage}{ExceptionStackTrace}",
                    ex.Message, ex.StackTrace
                );

                throw;
            }
        }

        private bool IsMessageWithEmptyData(ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event.Data.Length is not 0) return false;

            _logger.Information("Event without data received");
            
            return true;
        }

        private bool IsCheckpointMessage(ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event.EventType != MessageTypeMapper.ToName<Checkpoint>()) return false;

            _logger.Information("Checkpoint event - ignoring");
            
            return true;
        }

        private bool IsSubscribedToMessage(ResolvedEvent resolvedEvent)
        {
            Type eventType = MessageTypeMapper.ToType(resolvedEvent.Event.EventType);

            if (eventType is not null) return true;
            
            _logger.Information("Unknown event - ignoring");
            
            return false;
        }

        private void HandleDrop
        (
            StreamSubscription _,
            SubscriptionDroppedReason reason,
            Exception exception
        )
        {
            _logger.Warning
            (
                exception,
                "Subscription to all '{SubscriptionId}' dropped with '{Reason}'",
                _subscriptionConfig.SubscriptionName,
                reason
            );
            
            // Resubscribe if the client didn't stop the subscription
            if (reason is not SubscriptionDroppedReason.Disposed) Resubscribe();
        }

        private void Resubscribe()
        {
            while (true)
            {
                bool resubscribed = false;
                try
                {
                    Monitor.Enter(_resubscribeLock);

                    // Avoiding deadlocks: https://stackoverflow.com/questions/28305968/use-task-run-in-synchronous-method-to-avoid-deadlock-waiting-on-async-method
                    using (NoSynchronizationContextScope.Enter())
                    {
                        SubscribeToAllAsync(_cancellationTokenSource!.Token).Wait();
                    }

                    resubscribed = true;
                }
                catch (Exception exception)
                {
                    _logger.Warning
                    (
                        exception,
                        "Failed to resubscribe to all '{SubscriptionId}' dropped with '{ExceptionMessage}{ExceptionStackTrace}'",
                        _subscriptionConfig.SubscriptionName, exception.Message, exception.StackTrace
                    );
                }
                finally
                {
                    Monitor.Exit(_resubscribeLock);
                }

                if (resubscribed) break;

                Thread.Sleep(3000);
            }
        }
    }
}