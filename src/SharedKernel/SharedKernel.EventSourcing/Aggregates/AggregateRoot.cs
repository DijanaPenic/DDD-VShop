using System.Collections.Generic;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public abstract class AggregateRoot
    {
        private readonly List<IBaseEvent> _events = new();
        public IReadOnlyList<IBaseEvent> Events => _events;
        public bool IsRestored { get; private set; }
        public EntityId Id { get; protected set; }
        public int Version { get; private set; } = -1;

        protected abstract void ApplyEvent(IDomainEvent @event);

        protected void RaiseEvent(IDomainEvent @event)
        {
            ApplyEvent(@event);
            _events.Add(@event);
        }

        public void RaiseEvent(IIntegrationEvent @event) => _events.Add(@event);

        public void Restore() => IsRestored = true;

        public void Load(IEnumerable<IBaseEvent> history)
        {
            foreach (IBaseEvent @event in history)
            {
                if(@event is IDomainEvent domainEvent) ApplyEvent(domainEvent);
                Version++;
            }
        }

        public void Clear()
        {
            Version += _events.Count;
            _events.Clear();
        }

        protected void ApplyToEntity(IInternalEventHandler entity, IDomainEvent @event) => entity.Handle(@event);
    }
}