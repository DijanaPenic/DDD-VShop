using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using EventStore.Client;

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
        protected readonly EventStoreClient EventStoreClient;
        protected readonly string SubscriptionName;
        private readonly ISubscription[] _subscriptionHandlers;
        protected StreamSubscription EventStoreSubscription;
        
        private static readonly ILogger Logger = Log.ForContext<EventStoreAllFilteredCatchUpSubscriptionManager>();

        protected EventStoreSubscriptionManager
        (
            EventStoreClient eventStoreClient,
            ICheckpointRepository checkpointRepository,
            string subscriptionName,
            ISubscription[] subscriptionHandlers
        )
        {
            _subscriptionHandlers = subscriptionHandlers;

            EventStoreClient = eventStoreClient;
            SubscriptionName = $"{eventStoreClient.ConnectionName}{subscriptionName}"; // NOTE: Need to prefix with the name of the bounded context
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
            StreamSubscription _,
            ResolvedEvent resolvedEvent,
            CancellationToken cancellationToken
        )
        {
            if (resolvedEvent.Event.EventType.StartsWith("$")) return;

            IMessageMetadata metadata = resolvedEvent.DeserializeMetadata();
            IMessage message = resolvedEvent.DeserializeMessage();

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
        
        protected static Position GetPosition(ulong? checkpoint) 
        => checkpoint.HasValue
            ? new Position(checkpoint.Value, checkpoint.Value)
            : Position.Start;
    }
}