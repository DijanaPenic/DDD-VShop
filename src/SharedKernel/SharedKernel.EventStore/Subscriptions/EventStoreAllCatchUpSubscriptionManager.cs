using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.EventStore.Repositories.Contracts;
using VShop.SharedKernel.EventStore.Subscriptions.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Subscriptions
{
    public class EventStoreAllCatchUpSubscriptionManager : IEventStoreSubscriptionManager
    {
        private readonly IEventStoreCheckpointRepository _esCheckpointRepository;
        private readonly IEventStoreConnection _esConnection;
        private readonly ISubscription[] _subscriptionHandlers;
        private readonly string _esSubscriptionName;
        private EventStoreAllCatchUpSubscription _esSubscription;
        
        private static readonly ILogger Logger = Log.ForContext<EventStoreAllCatchUpSubscriptionManager>();

        public EventStoreAllCatchUpSubscriptionManager
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

        public async Task StartAsync()
        {
            CatchUpSubscriptionSettings settings = new
            (
                2000, 
                500,
                Logger.IsEnabled(LogEventLevel.Debug),
            false, 
                _esSubscriptionName
            );

            Logger.Debug("Starting the subscription manager...");

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
            ResolvedEvent resolvedEvent
        )
        {
            if (resolvedEvent.Event.EventType.StartsWith("$")) return;

            IMessage message = resolvedEvent.DeserializeData() as IMessage;
            MessageMetadata metadata = resolvedEvent.DeserializeMetadata();

            Logger.Debug("EventStore subscription manager > identified message: {Message}", message);

            try
            {
                await Task.WhenAll(_subscriptionHandlers.Select(sh => sh.ProjectAsync(message, metadata)));
                await _esCheckpointRepository.StoreCheckpointAsync(resolvedEvent.OriginalPosition?.CommitPosition);
            }
            catch (Exception ex)
            {
                Logger.Error
                (
                    ex,
                    "Error occured while projecting the message {Message}",
                    message
                );
                throw;
            }
        }

        public Task StopAsync()
        {
            _esSubscription.Stop();

            return Task.CompletedTask;
        }
    }
}