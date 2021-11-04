using Serilog;
using Serilog.Events;
using EventStore.ClientAPI;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing.Projections.Contracts;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Subscriptions
{
    public class EventStoreAllCatchUpSubscriptionManager : EventStoreSubscriptionManager
    {
        private static readonly ILogger Logger = Log.ForContext<EventStoreAllCatchUpSubscriptionManager>();

        public EventStoreAllCatchUpSubscriptionManager
        (
            IEventStoreConnection eventStoreConnection,
            ICheckpointRepository checkpointRepository,
            string subscriptionName,
            params ISubscription[] subscriptionHandlers
        ): base (eventStoreConnection, checkpointRepository, subscriptionName, subscriptionHandlers) { }

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

            long? checkpoint = await CheckpointRepository.GetCheckpointAsync();
            Logger.Debug("Retrieved the checkpoint {SubscriptionName}: {Checkpoint}", SubscriptionName, checkpoint);

            EventStoreSubscription = EventStoreConnection.SubscribeToAllFrom
            (
                GetPosition(checkpoint),
                settings,
                EventAppearedAsync
            );
            Logger.Debug("Subscribed to $all stream in {SubscriptionName}", SubscriptionName);
        }
    }
}