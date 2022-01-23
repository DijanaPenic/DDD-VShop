using Serilog;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Infrastructure.Threading;
using VShop.SharedKernel.EventStoreDb.Subscriptions.DAL;
using VShop.SharedKernel.EventStoreDb.Subscriptions.DAL.Entities;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Services.Contracts;

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
        private readonly string _subscriptionName;

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
            _subscriptionName = $"{eventStoreClient.ConnectionName}-{_subscriptionConfig.SubscriptionId}";
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation.
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _executingTask = SubscribeToAllAsync(_cancellationTokenSource.Token);

            return _executingTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start.
            if (_executingTask is null) return;

            // Signal cancellation to the executing method.
            _cancellationTokenSource.Cancel();

            // Wait until the issue completes or the stop token triggers.
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            // Throw if cancellation triggered.
            cancellationToken.ThrowIfCancellationRequested();

            _logger.Information("Subscription to all '{SubscriptionId}' stopped", _subscriptionName);
        }

        private async Task SubscribeToAllAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Subscription to all '{SubscriptionId}' started", _subscriptionName);

            using IServiceScope scope = _serviceProvider.CreateScope();
            SubscriptionDbContext subscriptionDbContext = scope.ServiceProvider.GetRequiredService<SubscriptionDbContext>();
            
            Checkpoint checkpoint = await subscriptionDbContext.Checkpoints
                .FirstOrDefaultAsync(c => c.SubscriptionId == _subscriptionName, cancellationToken);

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
                async Task CheckpointUpdate(SubscriptionDbContext subscriptionContext)
                {
                    Checkpoint checkpoint = await subscriptionContext.Checkpoints
                        .FirstOrDefaultAsync(c => c.SubscriptionId == _subscriptionName, cancellationToken);
            
                    ulong position = resolvedEvent.Event.Position.CommitPosition;

                    if (checkpoint is not null) checkpoint.Position = position;
                    else
                    {
                        subscriptionContext.Checkpoints.Add(new Checkpoint
                        {
                            SubscriptionId = _subscriptionName, 
                            Position = position
                        });
                    }

                    await subscriptionContext.SaveChangesAsync(cancellationToken);
                }
                await _subscriptionConfig.SubscriptionHandler.ProjectAsync(resolvedEvent, CheckpointUpdate, cancellationToken);
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

            _logger.Information("Event without data - ignoring");
            
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
            if (MessageTypeMapper.ToType(resolvedEvent.Event.EventType) is not null) return true;
            
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
                _subscriptionName, reason
            );
            
            // Resubscribe if the client didn't stop the subscription.
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
                        _subscriptionName, exception.Message, exception.StackTrace
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