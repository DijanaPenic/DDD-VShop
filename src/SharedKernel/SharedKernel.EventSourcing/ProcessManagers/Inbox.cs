using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class Inbox : IInbox
    {
        public IMessage Trigger { get; set; }
        public int Version { get; set; } = -1;
        public IDictionary<Type, Action<IBaseEvent>> EventHandlers { get; } 
            = new Dictionary<Type, Action<IBaseEvent>>();
        public IDictionary<Type, Action<IBaseCommand>> CommandHandlers { get; }
            = new Dictionary<Type, Action<IBaseCommand>>();
    }
    
    public interface IInbox
    {
        public IMessage Trigger { get; }
        public int Version { get; }
    }
}