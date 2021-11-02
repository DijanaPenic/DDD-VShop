using Serilog;
using Serilog.Events;
using EventStore.ClientAPI;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing;

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
            string subscriptionName,
            Filter subscriptionFilter,
            params ISubscription[] subscriptionHandlers
        ) : base(eventStoreConnection, subscriptionName, subscriptionHandlers)
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

            Logger.Debug("Starting the subscription manager {ESSubscriptionName}...", SubscriptionName);

            long? checkpoint = await CheckpointRepository.GetCheckpointAsync();
            Logger.Debug("Retrieved the checkpoint {ESSubscriptionName}: {Checkpoint}", SubscriptionName, checkpoint);

            EventStoreSubscription = EventStoreConnection.FilteredSubscribeToAllFrom
            (
                GetPosition(checkpoint),
                _subscriptionFilter,
                settings,
                EventAppearedAsync
            );
            Logger.Debug("Subscribed to filtered stream in {ESSubscriptionName}", SubscriptionName);
        }
    }
}