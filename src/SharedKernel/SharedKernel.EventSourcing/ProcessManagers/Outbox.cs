using System;
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
        private readonly List<IBaseCommand> _commands = new();
        private readonly List<IScheduledMessage> _scheduledMessages = new();
        
        public int Version { get; set; } = -1;

        public void Add(IBaseEvent @event) => _events.Add(@event);
        public void Add(IBaseCommand command) => _commands.Add(command);
        public void Add(IMessage message, DateTime scheduledTime)
            => _scheduledMessages.Add(new ScheduledMessage(message, scheduledTime));
        public IEnumerable<TEvent> GetEvents<TEvent>() => _events.OfType<TEvent>();
        public IEnumerable<IMessage> GetAllMessages()
            => _events.Concat<IMessage>(_commands).Concat(_scheduledMessages);
        public IEnumerable<IScheduledMessage> GetCommandsForDeferredDispatch()
            => _scheduledMessages.Where(m => m.Message is ICommand);
        public IEnumerable<IBaseCommand> GetCommandsForImmediateDispatch() => _commands;
    }
    
    public interface IOutbox
    {
        public int Version { get; }
        IEnumerable<TEvent> GetEvents<TEvent>();
        public IEnumerable<IMessage> GetAllMessages();
        public IEnumerable<IScheduledMessage> GetCommandsForDeferredDispatch();
        public IEnumerable<IBaseCommand> GetCommandsForImmediateDispatch();
    }
}