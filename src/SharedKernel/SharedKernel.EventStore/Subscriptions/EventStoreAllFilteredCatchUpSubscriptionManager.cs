using Serilog;
using Serilog.Events;
using System.Threading.Tasks;
using EventStore.Client;

using VShop.SharedKernel.EventSourcing.Projections;
using VShop.SharedKernel.EventSourcing.Repositories;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Subscriptions
{
    public class EventStoreAllFilteredCatchUpSubscriptionManager : EventStoreSubscriptionManager
    {
        private readonly Filter _subscriptionFilter;
        
        private static readonly ILogger Logger = Log.ForContext<EventStoreAllFilteredCatchUpSubscriptionManager>();

        public EventStoreAllFilteredCatchUpSubscriptionManager
        (
            EventStoreClient esClient,
            ICheckpointRepository checkpointRepository,
            string subscriptionName,
            Filter subscriptionFilter,
            params ISubscription[] subscriptionHandlers
        ) : base(esClient, checkpointRepository, subscriptionName, subscriptionHandlers)
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

            EventStoreSubscription = EventStoreClient.FilteredSubscribeToAllFrom
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