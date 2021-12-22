using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public abstract class AggregateRoot
    {
        private readonly List<IBaseEvent> _outbox = new();

        public EntityId Id { get; protected set; }
        public int Version { get; private set; } = -1;
        public Guid CorrelationId { get; init; }
        public Guid CausationId { get; init; }
        
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

        public IReadOnlyList<IDomainEvent> GetDomainEvents() => _outbox.OfType<IDomainEvent>().ToList();

        public IReadOnlyList<IBaseEvent> GetAllMessages() => _outbox;

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