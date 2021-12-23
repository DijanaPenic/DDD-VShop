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
        private readonly List<IBaseEvent> _outbox = new();

        public EntityId Id { get; protected set; }
        public int Version { get; private set; } = -1;
        public Guid CorrelationId { get; init; }
        public Guid CausationId { get; init; }
        
        protected abstract void ApplyEvent(IBaseEvent @event);

        protected void RaiseEvent(IBaseEvent @event)
        {
            ApplyEvent(@event);
            SetEventIdentification(@event);
            _outbox.Add(@event);
        }
        
        public void Load(IEnumerable<IBaseEvent> history)
        {
            (IEnumerable<IBaseEvent> pendingEvents, IEnumerable<IBaseEvent> processedEvents) = 
                history.Split(e => e.CausationId == CausationId);
            
            foreach (IBaseEvent @event in processedEvents)
            {
                ApplyEvent(@event);
                Version++;
            }
            foreach (IBaseEvent @event in pendingEvents) RaiseEvent(@event);
        }

        public IReadOnlyList<TType> GetOutboxMessages<TType>() => _outbox.OfType<TType>().ToList();

        public IReadOnlyList<IBaseEvent> GetOutboxMessages() => _outbox;

        public int OutboxMessageCount => _outbox.Count;

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