using NodaTime;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class ProcessManagerOutbox : IProcessManagerOutbox
    {
        private readonly List<IMessage> _messages = new();

        public int Version { get; set; } = -1;
        public int MessagesCount => GetMessages().Count;

        public void Add(IMessage message) => _messages.Add(message);
        
        public void Add(IMessage message, Instant scheduledTime)
            => _messages.Add(new ScheduledMessage(message, scheduledTime));
        
        public void Clear()
        {
            Version += _messages.Count;
            _messages.Clear();
        }
        
        public IReadOnlyList<IMessage> GetMessages() => _messages;

        public IReadOnlyList<IScheduledMessage> GetMessagesForDeferredDispatch()
            => _messages.OfType<IScheduledMessage>().ToList();
        
        public IReadOnlyList<IBaseCommand> GetCommandsForImmediateDispatch()
            => _messages.OfType<IBaseCommand>().ToList();
    }
    
    public interface IProcessManagerOutbox
    {
        public int Version { get; }
        public int MessagesCount { get; }
        public IReadOnlyList<IMessage> GetMessages();
        public IReadOnlyList<IScheduledMessage> GetMessagesForDeferredDispatch();
        public IReadOnlyList<IBaseCommand> GetCommandsForImmediateDispatch();
    }
}