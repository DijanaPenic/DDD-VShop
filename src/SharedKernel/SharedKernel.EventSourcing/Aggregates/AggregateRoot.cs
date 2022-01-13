using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public abstract class AggregateRoot
    {
        private readonly List<IBaseEvent> _queuedEvents = new();
        private readonly List<IIdentifiedEvent<IBaseEvent>> _restoredEvents = new();
        
        public IReadOnlyList<IBaseEvent> QueuedEvents => _queuedEvents;
        public IReadOnlyList<IIdentifiedEvent<IBaseEvent>> RestoredEvents => _restoredEvents;
        public bool IsRestored => _restoredEvents.Count > 0;
        public EntityId Id { get; protected set; }
        public int Version { get; private set; } = -1;

        protected abstract void ApplyEvent(IDomainEvent @event);

        protected void RaiseEvent(IDomainEvent @event)
        {
            ApplyEvent(@event);
            _queuedEvents.Add(@event);
        }

        public void RaiseEvent(IIntegrationEvent @event) => _queuedEvents.Add(@event);

        public void Restore(IEnumerable<IIdentifiedEvent<IBaseEvent>> history)
            => _restoredEvents.AddRange(history); 

        public void Load(IEnumerable<IIdentifiedEvent<IBaseEvent>> history)
        {
            foreach (IIdentifiedEvent<IBaseEvent> @event in history)
            {
                if(@event.Data is IDomainEvent domainEvent) ApplyEvent(domainEvent);
                Version++;
            }
        }

        public void Clear()
        {
            Version += _queuedEvents.Count;
            _queuedEvents.Clear();
            _restoredEvents.Clear();
        }

        protected void ApplyToEntity(IInternalEventHandler entity, IDomainEvent @event) => entity.Handle(@event);
    }
}