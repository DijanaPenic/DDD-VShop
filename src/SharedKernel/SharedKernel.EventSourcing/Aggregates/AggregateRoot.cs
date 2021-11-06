using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Events;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public abstract class AggregateRoot<TKey>
        where TKey : ValueObject
    {
        private readonly List<IMessage> _outgoingEvents = new();

        public TKey Id { get; protected set; }
        public int Version { get; private set; } = -1;
        public Guid CorrelationId { get; set; }
        public Guid MessageId { get; set; }

        protected abstract void When(IDomainEvent @event);

        protected void Apply(IDomainEvent @event)
        {
            When(@event);
            SetCausationId(@event);
            SetCorrelationId(@event);
            
            _outgoingEvents.Add(@event);
        }
        
        public void EnqueueEvents(params IIntegrationEvent[] events)
        {
            foreach (IIntegrationEvent @event in events)
            {
                SetCausationId(@event);
                SetCorrelationId(@event);
            }

            _outgoingEvents.AddRange(events);
        }

        public void Load(IEnumerable<IMessage> history)
        {
            foreach (IMessage @event in history)
            {
                if(@event is IDomainEvent domainEvent) When(domainEvent);
                Version++;
            }
        }

        public IEnumerable<IDomainEvent> GetOutgoingDomainEvents()
            => _outgoingEvents
                .Where(e => e is IDomainEvent)
                .Cast<IDomainEvent>();

        public IEnumerable<IMessage> GetAllMessages()
            => _outgoingEvents;

        public void ClearAllMessages()
            => _outgoingEvents.Clear();

        protected void ApplyToEntity(IInternalEventHandler entity, IDomainEvent @event)
            => entity?.Handle(@event);
        
        private void SetCausationId(IMessage @event)
            => @event.CausationId = MessageId;
        
        private void SetCorrelationId(IMessage @event)
            => @event.CorrelationId = CorrelationId;
    }
}