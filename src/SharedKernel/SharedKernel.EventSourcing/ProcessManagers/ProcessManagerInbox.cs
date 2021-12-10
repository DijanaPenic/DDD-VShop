using System;
using System.Collections.Generic;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class ProcessManagerInbox : IProcessManagerInbox
    {
        public IBaseEvent Trigger { get; set; }
        public int Version { get; set; } = -1;
        public IDictionary<Type, Action<IBaseEvent, IClockService>> EventHandlers { get; } 
            = new Dictionary<Type, Action<IBaseEvent, IClockService>>();
    }
    
    public interface IProcessManagerInbox
    {
        public IBaseEvent Trigger { get; }
        public int Version { get; }
    }
}