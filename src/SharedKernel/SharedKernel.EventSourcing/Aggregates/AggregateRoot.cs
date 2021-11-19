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
        public Guid CorrelationId { get; set; }
        public Guid MessageId { get; set; }
        
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

        public void Load(IEnumerable<IBaseEvent> history)
        {
            foreach (IBaseEvent @event in history)
            {
                if(@event is IDomainEvent domainEvent) ApplyEvent(domainEvent);
                Version++;
            }
        }

        public IEnumerable<IDomainEvent> GetOutgoingDomainEvents()
            => _outbox.OfType<IDomainEvent>();

        public IEnumerable<IBaseEvent> GetAllEvents()
            => _outbox;

        public void Clear()
            => _outbox.Clear();

        protected void ApplyToEntity(IInternalEventHandler entity, IDomainEvent @event)
            => entity?.Handle(@event);
        
        private void SetEventIdentification(IBaseEvent @event)
        {
            @event.CausationId = MessageId;
            @event.CorrelationId = CorrelationId;
        }
    }
}