using System;
using System.Collections.Generic;
using NodaTime;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class ProcessManagerInbox : IProcessManagerInbox
    {
        private readonly List<IBaseEvent> _events = new();
        
        public IReadOnlyList<IBaseEvent> Events => _events;
        public int Version { get; set; } = -1;
        public IDictionary<Type, Action<IBaseEvent, Instant>> MessageHandlers { get; }
            = new Dictionary<Type, Action<IBaseEvent, Instant>>();

        public void Add(IBaseEvent @event) => _events.Add(@event);
        public void Clear()
        {
            Version += _events.Count;
            _events.Clear();
        }
    }
    
    public interface IProcessManagerInbox
    {
        public int Version { get; }
        IReadOnlyList<IBaseEvent> Events { get; }
    }
}