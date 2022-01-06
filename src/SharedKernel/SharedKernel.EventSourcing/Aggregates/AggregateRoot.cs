using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public abstract class AggregateRoot
    {
        private readonly List<IBaseEvent> _events = new();
        
        public IReadOnlyList<IBaseEvent> Events => _events;
        public EntityId Id { get; protected set; }
        public int Version { get; private set; } = -1;
        public Guid CausationId { get; }
        public Guid CorrelationId { get; }

        protected AggregateRoot(Guid causationId, Guid correlationId)
        {
            CausationId = causationId;
            CorrelationId = correlationId;
        }

        protected abstract void ApplyEvent(IDomainEvent @event);

        protected void RaiseEvent(IDomainEvent @event)
        {
            ApplyEvent(@event);
            SetEventIdentification(@event);
            _events.Add(@event);
        }
        
        public void RaiseEvent(IIntegrationEvent @event)
        {
            SetEventIdentification(@event);
            _events.Add(@event);
        }

        // TODO - review this approach.
        public void Load(IEnumerable<IBaseEvent> history)
        {
            // Truncate events following the specified causationId.
            IList<IBaseEvent> historyList = history.ToList()
                .RemoveRangeAfterLast(e => e.CausationId == CausationId);
            
            // Restore aggregate state (identified by causationId param).
            (IEnumerable<IBaseEvent> pendingEvents, IEnumerable<IBaseEvent> processedEvents) = 
                historyList.Split(e => e.CausationId == CausationId);
            
            foreach (IBaseEvent @event in processedEvents)
            {
                if(@event is IDomainEvent domainEvent) ApplyEvent(domainEvent);
                Version++;
            }

            foreach (IBaseEvent @event in pendingEvents)
            {
                switch (@event)
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
        
        private void SetEventIdentification(IBaseEvent @event)
        {
            @event.CausationId = CausationId;
            @event.CorrelationId = CorrelationId;
        }
    }
}