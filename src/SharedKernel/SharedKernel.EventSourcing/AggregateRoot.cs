using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventSourcing
{
    public abstract class AggregateRoot<TKey>
        where TKey : ValueObject
    {
        private readonly IList<IDomainEvent> _changes = new List<IDomainEvent>();

        public TKey Id { get; protected set; }

        public int Version { get; private set; } = -1;

        protected abstract void When(IDomainEvent @event);

        protected void Apply(IDomainEvent @event)
        {
            When(@event);
            _changes.Add(@event);
        }
        
        public void Load(IEnumerable<IDomainEvent> history)
        {
            foreach (IDomainEvent @event in history)
            {
                When(@event);
                Version++;
            }
        }
        
        public IEnumerable<IDomainEvent> GetChanges() => _changes.AsEnumerable();
        
        public void ClearChanges() => _changes.Clear();

        protected void ApplyToEntity(IInternalEventHandler entity, IDomainEvent @event) => entity?.Handle(@event);
    }
}