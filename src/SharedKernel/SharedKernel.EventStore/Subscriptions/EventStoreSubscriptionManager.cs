using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.EventStore.Repositories;
using VShop.SharedKernel.EventStore.Repositories.Contracts;
using VShop.SharedKernel.EventStore.Subscriptions.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Subscriptions
{
    public abstract class EventStoreSubscriptionManager : IEventStoreSubscriptionManager
    {
        private readonly ISubscription[] _esSubscriptionHandlers;
        private static readonly ILogger Logger = Log.ForContext<EventStoreAllFilteredCatchUpSubscriptionManager>();
        
        protected readonly IEventStoreCheckpointRepository ESCheckpointRepository;
        protected readonly IEventStoreConnection ESConnection;
        protected readonly string ESSubscriptionName;
        protected EventStoreCatchUpSubscription ESSubscription;

        protected EventStoreSubscriptionManager
        (
            IEventStoreConnection esConnection, 
            string esSubscriptionName,
            ISubscription[] esSubscriptionHandlers
        )
        {
            _esSubscriptionHandlers = esSubscriptionHandlers;

            ESConnection = esConnection;
            ESSubscriptionName = $"{esConnection.ConnectionName}{esSubscriptionName}"; // Need to prefix with the name of the bounded context
            ESCheckpointRepository = new EventStoreCheckpointRepository(esConnection, esSubscriptionName);
        }
        
        public abstract Task StartAsync();
        
        public Task StopAsync()
        {
            ESSubscription.Stop();

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
            MessageMetadata metadata = resolvedEvent.DeserializeMetadata();

            Logger.Debug("EventStore subscription manager > identified message: {Message}", message);

            try
            {
                await Task.WhenAll(_esSubscriptionHandlers.Select(sh => sh.ProjectAsync(message, metadata)));
                await ESCheckpointRepository.StoreCheckpointAsync(resolvedEvent.OriginalPosition?.CommitPosition);
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
    }
}