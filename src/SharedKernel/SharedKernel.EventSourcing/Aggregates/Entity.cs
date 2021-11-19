using System;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public abstract class Entity<TId> : IInternalEventHandler
        where TId : ValueObject
    {
        private readonly Action<IDomainEvent> _applier;
        
        public TId Id { get; protected set; }

        protected Entity(Action<IDomainEvent> applier) => _applier = applier;

        void IInternalEventHandler.Handle(IDomainEvent @event) => ApplyEvent(@event);

        protected abstract void ApplyEvent(IDomainEvent @event);

        protected void RaiseEvent(IDomainEvent @event) => _applier(@event);
    }
}