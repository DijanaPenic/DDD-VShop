using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing;

namespace VShop.SharedKernel.EventStore
{
    public class EventStoreSubscriptionManager
    {
        // TODO - enable logging
        //static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly IEventStoreCheckpointRepository _esCheckpointRepository;
        private readonly IEventStoreConnection _esConnection;
        private readonly ISubscription[] _subscriptionHandlers;
        private readonly string _esSubscriptionName;
        private EventStoreAllCatchUpSubscription _esSubscription;

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
                false, //Log.IsDebugEnabled(),
                false, 
                _esSubscriptionName
            );

            //Log.Debug("Starting the projection manager...");

            long? position = await _esCheckpointRepository.GetCheckpointAsync();
            //Log.Debug("Retrieved the checkpoint: {checkpoint}", position);

            _esSubscription = _esConnection.SubscribeToAllFrom
            (
                GetPosition(),
                settings,
                EventAppearedAsync
            );
            //Log.Debug("Subscribed to $all stream");

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

            //Log.Debug("Projecting event {event}", @event.ToString());

            try
            {
                await Task.WhenAll(_subscriptionHandlers.Select(sh => sh.ProjectAsync(@event)));
                await _esCheckpointRepository.StoreCheckpointAsync(resolvedEvent.OriginalPosition?.CommitPosition);
            }
            catch (Exception e)
            {
                // Log.Error
                // (
                //     e,
                //     "Error occured when projecting the event {event}",
                //     @event
                // );
                throw;
            }
        }

        public void Stop() => _esSubscription.Stop();
    }
}