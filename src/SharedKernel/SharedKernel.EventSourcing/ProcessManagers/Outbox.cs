using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Messaging.Commands;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class Outbox : IOutbox
    {
        private readonly List<IBaseEvent> _events = new();
        private readonly List<IBaseCommand> _immediateCommands = new();
        private readonly List<IBaseCommand> _scheduledCommands = new();
        
        public int Version { get; set; } = -1;

        public void Raise(IBaseEvent @event) => _events.Add(@event);
        public void Raise(IBaseCommand command) => _immediateCommands.Add(command);
        public void Schedule(IBaseCommand command) => _scheduledCommands.Add(command);
        public IEnumerable<TEvent> GetEvents<TEvent>() => _events.OfType<TEvent>();
        public IEnumerable<IMessage> GetAllMessages()
            => _events.Concat<IMessage>(_immediateCommands).Concat(_scheduledCommands);
        public IEnumerable<IBaseCommand> GetScheduledCommands() => _scheduledCommands;
        public IEnumerable<IBaseCommand> GetImmediateCommands() => _immediateCommands;
    }
    
    public interface IOutbox
    {
        public int Version { get; }
        IEnumerable<TEvent> GetEvents<TEvent>();
        public IEnumerable<IMessage> GetAllMessages();
        public IEnumerable<IBaseCommand> GetScheduledCommands();
        public IEnumerable<IBaseCommand> GetImmediateCommands();
    }
}