﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using EventStore.ClientAPI;

using VShop.SharedKernel.Domain;
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

            IDomainEvent eventData = resolvedEvent.DeserializeData();
            EventMetadata eventMetadata = resolvedEvent.DeserializeMetadata();

            Logger.Debug("EventStore subscription manager > identified event: {EventData}", eventData);

            try
            {
                await Task.WhenAll(_subscriptionHandlers.Select(sh => sh.ProjectAsync(eventData, eventMetadata)));
                await _esCheckpointRepository.StoreCheckpointAsync(resolvedEvent.OriginalPosition?.CommitPosition);
            }
            catch (Exception ex)
            {
                Logger.Error
                (
                    ex,
                    "Error occured while projecting the event {EventData}",
                    eventData
                );
                throw;
            }
        }

        public void Stop() => _esSubscription.Stop();
    }
}