using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore
{
    public class EventStoreSubscriptionManager
    {
        private readonly IEventStoreCheckpointRepository _esCheckpointRepository;
        private readonly IEventStoreConnection _esConnection;
        private readonly ISubscription[] _subscriptionHandlers;
        private readonly string _esSubscriptionName;
        private EventStoreAllCatchUpSubscription _esSubscription;
        
        private static readonly ILogger Logger = Log.ForContext<EventStoreSubscriptionManager>();

        public EventStoreSubscriptionManager
        (
            IEventStoreConnection esConnection,
            IEventStoreCheckpointRepository esCheckpointRepository,
            string esSubscriptionName,
            params ISubscription[] subscriptionHandlers
        )
        {
            _esConnection = esConnection;
            _esCheckpointRepository = esCheckpointRepository;
            _esSubscriptionName = esSubscriptionName;
            _subscriptionHandlers = subscriptionHandlers;
        }

        public async Task Start()
        {
            CatchUpSubscriptionSettings settings = new
            (
                2000, 
                500,
                Logger.IsEnabled(LogEventLevel.Debug),
            false, 
                _esSubscriptionName
            );

            Logger.Debug("Starting the projection manager...");

            long? position = await _esCheckpointRepository.GetCheckpointAsync();
            Logger.Debug("Retrieved the checkpoint: {Checkpoint}", position);

            _esSubscription = _esConnection.SubscribeToAllFrom
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

        private async Task EventAppearedAsync
        (
            EventStoreCatchUpSubscription _,
            ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event.EventType.StartsWith("$")) return;

            object @event = resolvedEvent.Deserialize();

            Logger.Debug("Projecting event {Event}", @event.ToString());

            try
            {
                await Task.WhenAll(_subscriptionHandlers.Select(sh => sh.ProjectAsync(@event)));
                await _esCheckpointRepository.StoreCheckpointAsync(resolvedEvent.OriginalPosition?.CommitPosition);
            }
            catch (Exception e)
            {
                Logger.Error
                (
                    e,
                    "Error occured when projecting the event {Event}",
                    @event
                );
                throw;
            }
        }

        public void Stop() => _esSubscription.Stop();
    }
}