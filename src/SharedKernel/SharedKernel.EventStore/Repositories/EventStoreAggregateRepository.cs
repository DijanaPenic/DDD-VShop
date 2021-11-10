using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using EventStore.Client;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.EventSourcing.Aggregates;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Messaging.Events.Publishing;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Repositories
{
    public class EventStoreAggregateRepository<TA, TKey> : IAggregateRepository<TA, TKey>
        where TKey : ValueObject
        where TA : AggregateRoot<TKey>
    {
        private readonly EventStoreClient _eventStoreClient;
        private readonly Publisher _publisher;

        private static readonly ILogger Logger = Log.ForContext<EventStoreAggregateRepository<TA, TKey>>();

        public EventStoreAggregateRepository
        (
            EventStoreClient eventStoreClient,
            Publisher publisher
        )
        {
            _eventStoreClient = eventStoreClient;
            _publisher = publisher;
        }

        public async Task SaveAsync(TA aggregate, CancellationToken cancellationToken = default)
        {
            if (aggregate is null)
                throw new ArgumentNullException(nameof(aggregate));

            string streamName = GetAggregateStreamName(aggregate.Id);

            await _eventStoreClient.AppendToStreamWithRetryAsync
            (
                streamName,
                aggregate.Version,
                cancellationToken,
                aggregate.GetAllEvents().ToArray()
            );

            try
            {
                // https://stackoverflow.com/questions/59320296/how-to-add-mediatr-publishstrategy-to-existing-project
                foreach (IDomainEvent domainEvent in aggregate.GetOutgoingDomainEvents())
                    await _publisher.Publish(domainEvent, PublishStrategy.SyncStopOnException);
            }
            finally
            {
                aggregate.ClearAllEvents();
            }
        }

        public async Task<TA> LoadAsync
        (
            TKey aggregateId,
            Guid? messageId = null,
            Guid? correlationId = null,
            CancellationToken cancellationToken = default
        )
        {
            string streamName = GetAggregateStreamName(aggregateId);
            
            IEnumerable<IEvent> events = await _eventStoreClient.ReadStreamForwardAsync<IEvent>
            (
                streamName,
                StreamPosition.Start,
                cancellationToken
            );

            if (events.Count() is 0) return default; // TODO - zasto me Resharper tu upozorava?
                
            TA aggregate = (TA)Activator.CreateInstance(typeof(TA), true);
            if (aggregate is null)
                throw new Exception($"Couldn't resolve {nameof(aggregate)} instance.");
            
            if (messageId is not null) aggregate.MessageId = messageId.Value;
            if (correlationId is not null) aggregate.CorrelationId = correlationId.Value;
            
            aggregate?.Load(events);

            return aggregate;
        }

        private string GetAggregateStreamName(TKey aggregateId)
            => $"{_eventStoreClient.ConnectionName}/aggregate/{typeof(TA).Name}/{aggregateId}".ToSnakeCase();
    }
}