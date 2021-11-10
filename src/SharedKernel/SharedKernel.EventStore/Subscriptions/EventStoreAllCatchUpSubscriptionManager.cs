using Serilog;
using Serilog.Events;
using System.Threading.Tasks;
using EventStore.Client;

using VShop.SharedKernel.EventSourcing.Projections;
using VShop.SharedKernel.EventSourcing.Repositories;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Subscriptions
{
    public class EventStoreAllCatchUpSubscriptionManager : EventStoreSubscriptionManager
    {
        private static readonly ILogger Logger = Log.ForContext<EventStoreAllCatchUpSubscriptionManager>();

        public EventStoreAllCatchUpSubscriptionManager
        (
            EventStoreClient eventStoreClient,
            ICheckpointRepository checkpointRepository,
            string subscriptionName,
            params ISubscription[] subscriptionHandlers
        ): base (eventStoreClient, checkpointRepository, subscriptionName, subscriptionHandlers) { }

        public override async Task StartAsync()
        {
            CatchUpSubscriptionSettings settings = new
            (
                2000, 
                500,
                Logger.IsEnabled(LogEventLevel.Debug),
                false, 
                SubscriptionName
            );

            Logger.Debug("Starting the subscription manager {SubscriptionName}...", SubscriptionName);

            ulong? checkpoint = await CheckpointRepository.GetCheckpointAsync();
            Logger.Debug("Retrieved the checkpoint {SubscriptionName}: {Checkpoint}", SubscriptionName, checkpoint);

            EventStoreSubscription = EventStoreClient.SubscribeToAllFrom
            (
                GetPosition(checkpoint),
                settings,
                EventAppearedAsync
            );
            
            Position position = GetPosition(checkpoint);
            await EventStoreClient.SubscribeToAllAsync
            (
                position,
                eventAppeared: async (subscription, message, cancellationToken) => {
                    Logger.Debug
                    (
                        "Received event {OriginalEventNumber}@{OriginalStreamId}",
                        message.OriginalEventNumber, message.OriginalStreamId
                    );
                    
                    await EventAppearedAsync(subscription, message, cancellationToken);
                    position = message.OriginalPosition.Value;
                },
                subscriptionDropped: ((subscription, reason, exception) => {
                    Logger.Error(exception, "Subscription was dropped due to {Reason}", reason);
                    
                    // Resubscribe if the client didn't stop the subscription
                    if (reason != SubscriptionDroppedReason.Disposed) {
                        Resubscribe(position);
                    }
                })
            );
            
            Logger.Debug("Subscribed to $all stream in {SubscriptionName}", SubscriptionName);
        }
    }
}