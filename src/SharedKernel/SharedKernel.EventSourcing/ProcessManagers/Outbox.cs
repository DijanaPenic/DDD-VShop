using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Messaging.Commands;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class Outbox : IOutbox
    {
        private readonly List<IEvent> _events = new();
        private readonly List<ICommand> _immediateCommands = new();
        private readonly List<ICommand> _scheduledCommands = new();
        
        public int Version { get; set; } = -1;

        public void Raise(IEvent @event) => _events.Add(@event);
        public void Raise(ICommand command) => _immediateCommands.Add(command);
        public void Schedule(ICommand command) => _scheduledCommands.Add(command);
        public IEnumerable<TEvent> GetEvents<TEvent>() => _events.OfType<TEvent>();
        public IEnumerable<IMessage> GetAllMessages()
            => _events.Concat<IMessage>(_immediateCommands).Concat(_scheduledCommands);
        public IEnumerable<ICommand> GetScheduledCommands() => _scheduledCommands;
        public IEnumerable<ICommand> GetImmediateCommands() => _immediateCommands;
    }
    
    public interface IOutbox
    {
        public int Version { get; }
        IEnumerable<TEvent> GetEvents<TEvent>();
        public IEnumerable<IMessage> GetAllMessages();
        public IEnumerable<ICommand> GetScheduledCommands();
        public IEnumerable<ICommand> GetImmediateCommands();
    }
}