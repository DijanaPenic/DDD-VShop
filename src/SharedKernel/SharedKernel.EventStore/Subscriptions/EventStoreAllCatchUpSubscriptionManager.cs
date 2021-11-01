using Serilog;
using Serilog.Events;
using EventStore.ClientAPI;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.EventStore.Subscriptions.Contracts;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Subscriptions
{
    public class EventStoreAllCatchUpSubscriptionManager : EventStoreSubscriptionManager
    {
        private static readonly ILogger Logger = Log.ForContext<EventStoreAllCatchUpSubscriptionManager>();

        public EventStoreAllCatchUpSubscriptionManager
        (
            IEventStoreConnection esConnection,
            string esSubscriptionName,
            params ISubscription[] esSubscriptionHandlers
        ): base (esConnection, esSubscriptionName, esSubscriptionHandlers) { }

        public override async Task StartAsync()
        {
            CatchUpSubscriptionSettings settings = new
            (
                2000, 
                500,
                Logger.IsEnabled(LogEventLevel.Debug),
            false, 
                ESSubscriptionName
            );

            Logger.Debug("Starting the subscription manager...");

            long? position = await ESCheckpointRepository.GetCheckpointAsync();
            Logger.Debug("Retrieved the checkpoint: {Checkpoint}", position);

            ESSubscription = ESConnection.SubscribeToAllFrom
            (
                GetPosition(),
                settings,
                EventAppearedAsync
            );
            Logger.Debug("Subscribed to $all stream");

            Position? GetPosition() => position.HasValue
                ? new Position(position.Value, position.Value)
                : AllCheckpoint.AllStart;
        }
    }
}