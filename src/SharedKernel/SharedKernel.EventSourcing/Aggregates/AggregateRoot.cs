using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public abstract class AggregateRoot<TKey>
        where TKey : ValueObject
    {
        private readonly List<IBaseEvent> _outbox = new();

        public TKey Id { get; protected set; }
        public int Version { get; private set; } = -1;
        public Guid CorrelationId { get; set; } // Public - will be set when creating a new aggregate
        public Guid CausationId { get; set; }
        
        protected abstract void ApplyEvent(IDomainEvent @event);
        
        protected void RaiseEvent(IDomainEvent @event)
        {
            ApplyEvent(@event);
            SetEventIdentification(@event);
            _outbox.Add(@event);
        }
        
        public void RaiseEvent(IIntegrationEvent @event)
        {
            SetEventIdentification(@event);
            _outbox.Add(@event);
        }

        public void Load(IEnumerable<IBaseEvent> history, Guid? messageId, Guid? correlationId)
        {
            CausationId = messageId?? Guid.Empty;
            CorrelationId = correlationId?? Guid.Empty;

            foreach (IBaseEvent @event in history)
            {
                if(@event is IDomainEvent domainEvent) ApplyEvent(domainEvent);
                Version++;
            }
        }

        public IEnumerable<IDomainEvent> GetDomainEvents() => _outbox.OfType<IDomainEvent>();

        public IEnumerable<IBaseEvent> GetAllEvents() => _outbox;

        public void Clear()
        {
            Version += _outbox.Count;
            _outbox.Clear();
        }

        protected void ApplyToEntity(IInternalEventHandler entity, IDomainEvent @event) => entity.Handle(@event);
        
        private void SetEventIdentification(IBaseEvent @event)
        {
            @event.CausationId = CausationId;
            @event.CorrelationId = CorrelationId;
        }
    }
}