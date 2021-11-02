using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventSourcing
{
    public abstract class AggregateRoot<TKey>
        where TKey : ValueObject
    {
        private readonly IList<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        private readonly IList<IIntegrationEvent> _integrationEvents = new List<IIntegrationEvent>();

        public TKey Id { get; protected set; }

        public int Version { get; private set; } = -1;

        protected abstract void When(IDomainEvent @event);

        protected void Apply(IDomainEvent @event)
        {
            When(@event);
            _domainEvents.Add(@event);
        }
        
        public void Apply(IIntegrationEvent @event)
        {
            _integrationEvents.Add(@event);
        }
        
        public void Load(IEnumerable<IMessage> history)
        {
            foreach (IMessage message in history)
            {
                if(message is IDomainEvent domainEvent) When(domainEvent);
                Version++;
            }
        }

        public IEnumerable<IDomainEvent> GetDomainEvents() => _domainEvents.ToArray();
        public IEnumerable<IIntegrationEvent> GetIntegrationEvents() => _integrationEvents.AsEnumerable();

        public void ClearEvents()
        {
            _domainEvents.Clear();
            _integrationEvents.Clear();
        }

        protected void ApplyToEntity(IInternalEventHandler entity, IDomainEvent @event) => entity?.Handle(@event);
    }
}