using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Messaging.Commands;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class Outbox
    {
        public int Version { get; set; } = -1;
        public List<IEvent> Events { get; } = new();
        public List<ICommand> Commands { get; } = new();
        
        public IEnumerable<T> GetEvents<T>()
            => Events.OfType<T>();
        
        public IEnumerable<IMessage> GetMessages()
            => Events.Concat<IMessage>(Commands);
    }
}