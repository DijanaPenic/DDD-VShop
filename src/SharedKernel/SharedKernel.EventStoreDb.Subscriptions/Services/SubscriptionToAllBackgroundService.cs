﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.EventStoreDb.Messaging;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Repositories;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Repositories.Contracts;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Threading;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStoreDb.Subscriptions.Services
{
    public class SubscriptionToAllBackgroundService : ISubscriptionBackgroundService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task _executingTask;
        private readonly object _resubscribeLock = new();
        
        private readonly EventStoreClient _eventStoreClient;
        private readonly ICheckpointRepository _checkpointRepository;
        private readonly string _subscriptionId;
        private readonly ISubscriptionHandler[] _subscriptionHandlers;
        private readonly SubscriptionFilterOptions _filterOptions;

        private static readonly ILogger Logger = Log.ForContext<SubscriptionToAllBackgroundService>();

        public SubscriptionToAllBackgroundService
        (
            EventStoreClient eventStoreClient,
            ICheckpointRepository checkpointRepository,
            string subscriptionId,
            ISubscriptionHandler[] subscriptionHandlers,
            SubscriptionFilterOptions filterOptions = default
        )
        {
            _eventStoreClient = eventStoreClient;
            _subscriptionId = $"{eventStoreClient.ConnectionName}-{subscriptionId}";
            _subscriptionHandlers = subscriptionHandlers;
            _checkpointRepository = checkpointRepository;
            _filterOptions = filterOptions ?? new SubscriptionFilterOptions(EventTypeFilter.ExcludeSystemEvents());
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

            Logger.Information("Subscription to all '{SubscriptionId}' stopped", _subscriptionId);
        }

        private async Task SubscribeToAllAsync(CancellationToken cancellationToken)
        {
            Logger.Information("Subscription to all '{SubscriptionId}' started", _subscriptionId);

            ulong? checkpoint = await _checkpointRepository.LoadAsync(_subscriptionId, cancellationToken);

            await _eventStoreClient.SubscribeToAllAsync
            (
                GetPosition(checkpoint),
                HandleEventAsync,
                false,
                HandleDrop,
                _filterOptions,
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

            IMessageMetadata metadata = resolvedEvent.DeserializeMetadata();
            IMessage message = resolvedEvent.DeserializeData<IMessage>();

            try
            {
                await Task.WhenAll(_subscriptionHandlers.Select(sh => sh.ProjectAsync
                (
                    message,
                    metadata,
                    cancellationToken
                )));
                await _checkpointRepository.SaveAsync
                (
                    _subscriptionId,
                    resolvedEvent.Event.Position.CommitPosition,
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                Logger.Error
                (
                    ex,
                    "Error consuming message: {ExceptionMessage}{ExceptionStackTrace}",
                    ex.Message, ex.StackTrace
                );
            }
        }
        
        private static bool IsMessageWithEmptyData(ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event.Data.Length is not 0) return false;

            Logger.Information("Event without data received");
            
            return true;
        }

        private static bool IsCheckpointMessage(ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event.EventType != MessageTypeMapper.ToName<Checkpoint>()) return false;

            Logger.Information("Checkpoint event - ignoring");
            
            return true;
        }

        private static bool IsSubscribedToMessage(ResolvedEvent resolvedEvent)
        {
            Type eventType = MessageTypeMapper.ToType(resolvedEvent.Event.EventType);

            if (eventType is not null) return true;
            
            Logger.Information("Unknown event - ignoring");
            
            return false;
        }

        private void HandleDrop
        (
            StreamSubscription _,
            SubscriptionDroppedReason reason,
            Exception exception
        )
        {
            Logger.Warning
            (
                exception,
                "Subscription to all '{SubscriptionId}' dropped with '{Reason}'",
                _subscriptionId,
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
                    Logger.Warning
                    (
                        exception,
                        "Failed to resubscribe to all '{SubscriptionId}' dropped with '{ExceptionMessage}{ExceptionStackTrace}'",
                        _subscriptionId, exception.Message, exception.StackTrace
                    );
                }
                finally
                {
                    Monitor.Exit(_resubscribeLock);
                }

                if (resubscribed) break;

                Thread.Sleep(1000);
            }
        }
    }
}