using Serilog;
using Serilog.Events;
using EventStore.ClientAPI;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.EventStore.Subscriptions.Contracts;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Subscriptions
{
    public class EventStoreAllFilteredCatchUpSubscriptionManager : EventStoreSubscriptionManager
    {
        private static readonly ILogger Logger = Log.ForContext<EventStoreAllFilteredCatchUpSubscriptionManager>();
        private readonly Filter _esSubscriptionFilter;

        public EventStoreAllFilteredCatchUpSubscriptionManager
        (
            IEventStoreConnection esConnection,
            string esSubscriptionName,
            Filter esSubscriptionFilter,
            params ISubscription[] esSubscriptionHandlers
        ) : base(esConnection, esSubscriptionName, esSubscriptionHandlers)
        {
            _esSubscriptionFilter = esSubscriptionFilter;
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
                ESSubscriptionName
            );

            Logger.Debug("Starting the subscription manager...");

            long? position = await ESCheckpointRepository.GetCheckpointAsync();
            Logger.Debug("Retrieved the checkpoint: {Checkpoint}", position);

            //Regex regexExpression = new(@".*\/integration$"); // TODO - purge

            ESSubscription = ESConnection.FilteredSubscribeToAllFrom
            (
                GetPosition(),
                //Filter.EventType.Regex(regexExpression),  // TODO - purge
                _esSubscriptionFilter,
                settings,
                EventAppearedAsync
            );
            Logger.Debug("Subscribed to filtered stream");

            Position? GetPosition() => position.HasValue
                ? new Position(position.Value, position.Value)
                : AllCheckpoint.AllStart;
        }
    }
}