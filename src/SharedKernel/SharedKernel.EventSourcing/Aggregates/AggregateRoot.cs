using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.EventSourcing.Messaging;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public abstract class AggregateRoot<TKey>
        where TKey : ValueObject
    {
        private readonly List<IEvent> _outgoingEvents = new();

        public TKey Id { get; protected set; }
        public int Version { get; private set; } = -1;
        public Guid CorrelationId { get; set; }
        public Guid MessageId { get; set; }
        
        protected abstract void ApplyEvent(IDomainEvent @event);
        
        protected void RaiseEvent(IDomainEvent @event)
        {
            ApplyEvent(@event);
            SetEvent(@event);
            _outgoingEvents.Add(@event);
        }
        
        public void RaiseEvent(IIntegrationEvent @event)
        {
            SetEvent(@event);
            _outgoingEvents.Add(@event);
        }

        public void Load(IEnumerable<IEvent> history)
        {
            foreach (IEvent @event in history)
            {
                if(@event is IDomainEvent domainEvent) ApplyEvent(domainEvent);
                Version++;
            }
        }

        public IEnumerable<IDomainEvent> GetOutgoingDomainEvents()
            => _outgoingEvents.OfType<IDomainEvent>();

        public IEnumerable<IEvent> GetAllEvents()
            => _outgoingEvents;

        public void ClearAllEvents()
            => _outgoingEvents.Clear();

        protected void ApplyToEntity(IInternalEventHandler entity, IDomainEvent @event)
            => entity?.Handle(@event);
        
        private void SetEvent(IEvent @event)
        {
            @event.Name = MessageTypeMapper.ToName(@event.GetType());
            @event.MessageId = DeterministicGuid.Create(MessageId, $"{@event.Name}-{_outgoingEvents.Count}");
            @event.CausationId = MessageId;
            @event.CorrelationId = CorrelationId;
        }
    }
}