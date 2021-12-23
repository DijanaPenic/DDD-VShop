using NodaTime;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class ProcessManagerOutbox : IProcessManagerOutbox
    {
        private readonly List<IIntegrationEvent> _events = new();
        private readonly List<IBaseCommand> _commands = new();
        private readonly List<IScheduledMessage> _scheduledEvents = new();
        private readonly List<IScheduledMessage> _scheduledCommands = new();

        public int Version { get; set; }
        public int Count => GetMessages().Count;
        
        public ProcessManagerOutbox(int version = -1) => Version = version;

        public void Add(IIntegrationEvent @event) => _events.Add(@event);
        public void Add(IBaseCommand command) => _commands.Add(command);
        public void Add(IDomainEvent @event, Instant scheduledTime)
            => _scheduledEvents.Add(new ScheduledMessage(@event, scheduledTime));
        public void Add(IBaseCommand command, Instant scheduledTime)
            => _scheduledCommands.Add(new ScheduledMessage(command, scheduledTime));
        public IReadOnlyList<IMessage> GetMessages()
            => _events.Concat<IMessage>(_commands).Concat(_scheduledCommands).Concat(_scheduledEvents).ToList();
        public IReadOnlyList<IScheduledMessage> GetMessagesForDeferredDispatch() 
            => _scheduledEvents.Concat(_scheduledCommands).ToList();
        public IReadOnlyList<IBaseCommand> GetCommandsForImmediateDispatch() => _commands;
    }
    
    public interface IProcessManagerOutbox
    {
        public int Version { get; }
        public IReadOnlyList<IMessage> GetMessages();
        public IReadOnlyList<IScheduledMessage> GetMessagesForDeferredDispatch();
        public IReadOnlyList<IBaseCommand> GetCommandsForImmediateDispatch();
    }
}