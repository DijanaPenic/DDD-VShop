using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.EventStore.Subscriptions.Contracts;
using VShop.SharedKernel.EventSourcing.Messaging;
using VShop.SharedKernel.EventSourcing.Projections;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.Infrastructure.Messaging;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Subscriptions
{
    public abstract class EventStoreSubscriptionManager : IEventStoreSubscriptionManager
    {
        protected readonly ICheckpointRepository CheckpointRepository;
        protected readonly IEventStoreConnection EventStoreConnection;
        protected readonly string SubscriptionName;
        private readonly ISubscription[] _subscriptionHandlers;
        protected EventStoreCatchUpSubscription EventStoreSubscription;
        
        private static readonly ILogger Logger = Log.ForContext<EventStoreAllFilteredCatchUpSubscriptionManager>();

        protected EventStoreSubscriptionManager
        (
            IEventStoreConnection eventStoreConnection,
            ICheckpointRepository checkpointRepository,
            string subscriptionName,
            ISubscription[] subscriptionHandlers
        )
        {
            _subscriptionHandlers = subscriptionHandlers;

            EventStoreConnection = eventStoreConnection;
            SubscriptionName = $"{eventStoreConnection.ConnectionName}{subscriptionName}"; // NOTE: Need to prefix with the name of the bounded context
            CheckpointRepository = checkpointRepository;
        }
        
        public abstract Task StartAsync();
        
        public Task StopAsync()
        {
            EventStoreSubscription.Stop();

            return Task.CompletedTask;
        }
        
        protected async Task EventAppearedAsync
        (
            EventStoreCatchUpSubscription _,
            ResolvedEvent resolvedEvent
        )
        {
            if (resolvedEvent.Event.EventType.StartsWith("$")) return;

            IMessage message = resolvedEvent.DeserializeData() as IMessage;
            IMessageMetadata metadata = resolvedEvent.DeserializeMetadata();

            Logger.Debug("EventStore subscription manager > identified message: {Message}", message);

            try
            {
                await Task.WhenAll(_subscriptionHandlers.Select(sh => sh.ProjectAsync(message, metadata)));
                await CheckpointRepository.StoreCheckpointAsync(resolvedEvent.OriginalPosition?.CommitPosition);
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
        
        protected static Position? GetPosition(long? checkpoint) 
        => checkpoint.HasValue
            ? new Position(checkpoint.Value, checkpoint.Value)
            : AllCheckpoint.AllStart;
    }
}