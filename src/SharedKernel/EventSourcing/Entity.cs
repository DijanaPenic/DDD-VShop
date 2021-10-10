using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.SharedKernel.EventSourcing
{
    public abstract class Entity : IInternalEventHandler
    {
        private readonly Action<object> _applier;
        
        public EntityId Id { get; protected set; }

        protected Entity(Action<object> applier) => _applier = applier;

        void IInternalEventHandler.Handle(object @event) => When(@event);

        protected abstract void When(object @event);

        protected void Apply(object @event)
        {
            When(@event);
            _applier(@event);
        }
    }
}