using NodaTime;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class ProcessManagerOutbox : IProcessManagerOutbox
    {
        private readonly List<IMessage> _queuedMessages = new();
        private readonly List<IIdentifiedMessage<IMessage>> _restoredMessages = new();
        
        public IReadOnlyList<IMessage> QueuedMessages => _queuedMessages;
        public IReadOnlyList<IIdentifiedMessage<IMessage>> RestoredMessages => _restoredMessages;
        public int Version { get; private set; } = -1;
        
        public void Add(IMessage message) 
            => _queuedMessages.Add(message);
        public void Add(IMessage message, Instant scheduledTime)
            => _queuedMessages.Add(new ScheduledMessage(message, scheduledTime));
        public void Restore(IEnumerable<IIdentifiedMessage<IMessage>> messages)
            => _restoredMessages.AddRange(messages);
        
        public void Clear()
        {
            Version += _queuedMessages.Count;
            _queuedMessages.Clear();
            _restoredMessages.Clear();
        }
    }
    
    public interface IProcessManagerOutbox
    {
        public int Version { get; }
        public IReadOnlyList<IMessage> QueuedMessages { get; }
        public IReadOnlyList<IIdentifiedMessage<IMessage>> RestoredMessages { get; }
    }
}