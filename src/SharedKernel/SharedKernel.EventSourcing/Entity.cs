using System;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventSourcing
{
    public abstract class Entity<TId> : IInternalEventHandler
        where TId : ValueObject
    {
        private readonly Action<IDomainEvent> _applier;
        
        public TId Id { get; protected set; }

        protected Entity(Action<IDomainEvent> applier) => _applier = applier;

        void IInternalEventHandler.Handle(IDomainEvent @event) => When(@event);

        protected abstract void When(IDomainEvent @event);

        protected void Apply(IDomainEvent @event) => _applier(@event);
    }
}