using Serilog;
using Serilog.Events;
using EventStore.ClientAPI;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing.Projections;
using VShop.SharedKernel.EventSourcing.Repositories;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Subscriptions
{
    public class EventStoreAllFilteredCatchUpSubscriptionManager : EventStoreSubscriptionManager
    {
        private static readonly ILogger Logger = Log.ForContext<EventStoreAllFilteredCatchUpSubscriptionManager>();
        private readonly Filter _subscriptionFilter;

        public EventStoreAllFilteredCatchUpSubscriptionManager
        (
            IEventStoreConnection eventStoreConnection,
            ICheckpointRepository checkpointRepository,
            string subscriptionName,
            Filter subscriptionFilter,
            params ISubscription[] subscriptionHandlers
        ) : base(eventStoreConnection, checkpointRepository, subscriptionName, subscriptionHandlers)
        {
            _subscriptionFilter = subscriptionFilter;
        }

        public override async Task StartAsync()
        {
            CatchUpSubscriptionFilteredSettings settings = new
            (
                2000, 
                500,
                Logger.IsEnabled(LogEventLevel.Debug),
            false, 
                1000,
                SubscriptionName
            );

            Logger.Debug("Starting the subscription manager {SubscriptionName}...", SubscriptionName);

            long? checkpoint = await CheckpointRepository.GetCheckpointAsync();
            Logger.Debug("Retrieved the checkpoint {SubscriptionName}: {Checkpoint}", SubscriptionName, checkpoint);

            EventStoreSubscription = EventStoreConnection.FilteredSubscribeToAllFrom
            (
                GetPosition(checkpoint),
                _subscriptionFilter,
                settings,
                EventAppearedAsync
            );
            Logger.Debug("Subscribed to filtered stream in {SubscriptionName}", SubscriptionName);
        }
    }
}