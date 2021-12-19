using System;
using System.Collections.Generic;
using NodaTime;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class ProcessManagerInbox : IProcessManagerInbox
    {
        public IBaseEvent Trigger { get; set; }
        public int Version { get; set; }
        public IDictionary<Type, Action<IBaseEvent>> EventHandlers { get; }
            = new Dictionary<Type, Action<IBaseEvent>>();
        public IDictionary<Type, Action<IBaseEvent, Instant>> ScheduledEventHandlers { get; }
            = new Dictionary<Type, Action<IBaseEvent, Instant>>();

        public ProcessManagerInbox(int version = -1) => Version = version;
    }
    
    public interface IProcessManagerInbox
    {
        public IBaseEvent Trigger { get; }
        public int Version { get; }
    }
}