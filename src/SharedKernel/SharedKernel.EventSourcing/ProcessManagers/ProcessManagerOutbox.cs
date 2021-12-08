using System;
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

        public int Version { get; set; } = -1;

        public void Add(IIntegrationEvent @event) => _events.Add(@event);
        public void Add(IBaseCommand command) => _commands.Add(command);
        public void Add(IDomainEvent @event, DateTime scheduledTime)
            => _scheduledEvents.Add(new ScheduledMessage(@event, scheduledTime));
        public void Add(IBaseCommand command, DateTime scheduledTime)
            => _scheduledCommands.Add(new ScheduledMessage(command, scheduledTime));
        public IEnumerable<IMessage> GetAllMessages()
            => _events.Concat<IMessage>(_commands).Concat(_scheduledCommands).Concat(_scheduledEvents);
        public IEnumerable<IScheduledMessage> GetCommandsForDeferredDispatch() => _scheduledCommands;
        public IEnumerable<IScheduledMessage> GetEventsForDeferredDispatch() => _scheduledEvents;
        public IEnumerable<IBaseCommand> GetCommandsForImmediateDispatch() => _commands;
    }
    
    public interface IProcessManagerOutbox
    {
        public int Version { get; }
        public IEnumerable<IMessage> GetAllMessages();
        public IEnumerable<IScheduledMessage> GetCommandsForDeferredDispatch();
        public IEnumerable<IScheduledMessage> GetEventsForDeferredDispatch();
        public IEnumerable<IBaseCommand> GetCommandsForImmediateDispatch();
    }
}