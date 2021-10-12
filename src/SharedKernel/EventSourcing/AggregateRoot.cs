using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.SharedKernel.EventSourcing
{
    public abstract class AggregateRoot<TKey>
        where TKey : ValueObject
    {
        private readonly IList<object> _changes = new List<object>();

        public TKey Id { get; protected set; }

        public int Version { get; private set; } = -1;

        protected abstract void When(object @event);

        protected void Apply(object @event)
        {
            When(@event);
            _changes.Add(@event);
        }
        
        public void Load(IEnumerable<object> history)
        {
            foreach (object @event in history)
            {
                When(@event);
                Version++;
            }
        }
        
        public IEnumerable<object> GetChanges() => _changes.AsEnumerable();
        
        public void ClearChanges() => _changes.Clear();

        protected void ApplyToEntity(IInternalEventHandler entity, object @event) => entity?.Handle(@event);
    }
}