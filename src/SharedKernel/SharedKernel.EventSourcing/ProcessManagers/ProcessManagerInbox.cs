using System;
using System.Collections.Generic;
using NodaTime;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class ProcessManagerInbox : IProcessManagerInbox
    {
        private readonly List<IBaseEvent> _events = new();
        public int Version { get; set; } = -1;
        public int EventsCount => GetEvents().Count;
        public IDictionary<Type, Action<IBaseEvent, Instant>> EventHandlers { get; }
            = new Dictionary<Type, Action<IBaseEvent, Instant>>();

        public void Add(IBaseEvent @event) => _events.Add(@event);

        public void Clear()
        {
            Version += _events.Count;
            _events.Clear();
        }
        
        public IReadOnlyList<IBaseEvent> GetEvents() => _events;
    }
    
    public interface IProcessManagerInbox
    {
        public int Version { get; }
        public int EventsCount { get; }
        IReadOnlyList<IBaseEvent> GetEvents();
    }
}