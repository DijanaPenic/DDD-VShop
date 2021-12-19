using System;
using System.Linq;
using System.Collections.Generic;
using NodaTime;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class ProcessManagerInbox : IProcessManagerInbox
    {
        private readonly List<IBaseEvent> _events = new();
        public int Version { get; set; }
        public IDictionary<Type, Action<IBaseEvent>> EventHandlers { get; }
            = new Dictionary<Type, Action<IBaseEvent>>();
        public IDictionary<Type, Action<IBaseEvent, Instant>> ScheduledEventHandlers { get; }
            = new Dictionary<Type, Action<IBaseEvent, Instant>>();

        public ProcessManagerInbox(int version = -1) => Version = version;

        public void Add(IBaseEvent @event) => _events.Add(@event);
        public IEnumerable<IBaseEvent> GetAllEvents() => _events;
        public int Count() => GetAllEvents().Count();
    }
    
    public interface IProcessManagerInbox
    {
        public int Version { get; }

        IEnumerable<IBaseEvent> GetAllEvents();

        int Count();
    }
}