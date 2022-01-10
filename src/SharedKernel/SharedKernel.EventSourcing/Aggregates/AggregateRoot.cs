using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Helpers;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public abstract class AggregateRoot
    {
        private readonly List<IIdentifiedEvent> _events = new();
        
        public IReadOnlyList<IIdentifiedEvent> Events => _events;
        public IReadOnlyList<IIdentifiedEvent> DomainEvents // TODO - refactor
            => _events.Where(ie => ie.Data is IDomainEvent).ToList();
        
        public EntityId Id { get; protected set; }
        public int Version { get; private set; } = -1;
        public Guid CausationId { get; private set; }
        public Guid CorrelationId { get; private set; }
        
        protected AggregateRoot() { }

        protected AggregateRoot(Guid causationId, Guid correlationId)
        {
            CausationId = causationId;
            CorrelationId = correlationId;
        }

        protected abstract void ApplyEvent(IDomainEvent @event);

        protected void RaiseEvent(IDomainEvent @event)
        {
            ApplyEvent(@event);
            _events.Add(new IdentifiedEvent(@event, GetMetadata()));
        }
        
        public void RaiseEvent(IIntegrationEvent @event)
            => _events.Add(new IdentifiedEvent(@event, GetMetadata()));
        
        public void Load(IEnumerable<IIdentifiedEvent> history, Guid causationId, Guid correlationId)
        {
            CausationId = causationId;
            CorrelationId = correlationId;
            
            // Truncate events following the specified causationId.
            IList<IIdentifiedEvent> historyList = history.ToList()
                .RemoveRangeFollowingLast(e => e.Metadata.CausationId == CausationId);
            
            // Restore aggregate state (identified by causationId param).
            (IEnumerable<IIdentifiedEvent> pendingEvents, IEnumerable<IIdentifiedEvent> processedEvents) = 
                historyList.Split(e => e.Metadata.CausationId == CausationId);
            
            foreach (IIdentifiedEvent @event in processedEvents)
            {
                if(@event.Data is IDomainEvent domainEvent) ApplyEvent(domainEvent);
                Version++;
            }

            foreach (IIdentifiedEvent @event in pendingEvents)
            {
                switch (@event.Data)
                {
                    case IDomainEvent domainEvent:
                        RaiseEvent(domainEvent);
                        break;
                    case IIntegrationEvent integrationEvent:
                        RaiseEvent(integrationEvent);
                        break;
                }
            }
        }

        public void Clear()
        {
            Version += _events.Count;
            _events.Clear();
        }

        protected void ApplyToEntity(IInternalEventHandler entity, IDomainEvent @event) => entity.Handle(@event);
        private MessageMetadata GetMetadata() => new(SequentialGuid.Create(), CorrelationId, CausationId);
    }
}