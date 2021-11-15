using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Messaging.Commands;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    // TODO - maybe use base class for inbox and outbox
    public class Inbox
    {
        public int Version { get; set; } = -1;
        public List<IEvent> Events { get; } = new();
        public List<ICommand> Commands { get; } = new();
        public IDictionary<Type, Action<IEvent>> EventHandlers { get; set; } = new Dictionary<Type, Action<IEvent>>();
        
        public IEnumerable<T> GetEvents<T>()
            => Events.OfType<T>();
        
        public IEnumerable<IMessage> GetMessages()
            => Events.Concat<IMessage>(Commands);
    }
}