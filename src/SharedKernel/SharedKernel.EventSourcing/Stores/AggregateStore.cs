using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.EventSourcing.Stores
{
    public class AggregateStore<TAggregate> : IAggregateStore<TAggregate> where TAggregate : AggregateRoot, new()
    {
        private readonly CustomEventStoreClient _eventStoreClient;
        private readonly IMessageContextRegistry _messageContextRegistry;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IContext _context;

        public AggregateStore
        (
            CustomEventStoreClient eventStoreClient,
            IMessageContextRegistry messageContextRegistry,
            IEventDispatcher eventDispatcher, 
            IContext context
        )
        {
            _eventStoreClient = eventStoreClient;
            _messageContextRegistry = messageContextRegistry;
            _eventDispatcher = eventDispatcher;
            _context = context;
        }

        public async Task SaveAndPublishAsync
        (
            TAggregate aggregate,
            CancellationToken cancellationToken = default
        )
        {
            if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

            await SaveAsync(aggregate, cancellationToken);

            try
            {
                await PublishAsync(aggregate.Events, cancellationToken);
            }
            finally
            {
                aggregate.Clear();
            }
        }

        public async Task SaveAsync
        (
            TAggregate aggregate,
            CancellationToken cancellationToken = default
        )
        {
            if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

            foreach (IBaseEvent @event in aggregate.Events)
                _messageContextRegistry.Set(@event, new MessageContext(_context));

            await _eventStoreClient.AppendToStreamAsync
            (
                GetStreamName(aggregate.Id),
                aggregate.Version,
                aggregate.Events,
                cancellationToken
            );
        }

        public async Task<TAggregate> LoadAsync
        (
            EntityId aggregateId,
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<MessageEnvelope<IBaseEvent>> messageEnvelopes = await _eventStoreClient
                .ReadStreamForwardAsync<IBaseEvent>(GetStreamName(aggregateId), cancellationToken);

            if (messageEnvelopes.Count is 0) return default;

            TAggregate aggregate = new();
            aggregate.Load(messageEnvelopes.ToMessages());
            
            IList<MessageEnvelope<IBaseEvent>> processed = messageEnvelopes
                .Where(e => e.MessageContext.Context.RequestId == _context.RequestId).ToList();

            if (!processed.Any()) return aggregate;
            
            foreach ((IBaseEvent @event, IMessageContext messageContext) in processed)
                _messageContextRegistry.Set(@event, messageContext);
            
            await PublishAsync(processed.ToMessages(), cancellationToken); 
            aggregate.Restore();

            return aggregate;
        }
        
        private async Task PublishAsync
        (
            IEnumerable<IBaseEvent> events,
            CancellationToken cancellationToken = default
        )
        {
            foreach (IBaseEvent @event in events)
            {
                await _eventDispatcher.PublishAsync
                (
                    @event,
                    EventDispatchStrategy.SyncStopOnException,
                    cancellationToken
                );
            }
        }

        public static string GetStreamName(EntityId aggregateId) 
            => $"aggregate/{typeof(TAggregate).Name}/{aggregateId}";
    }
}