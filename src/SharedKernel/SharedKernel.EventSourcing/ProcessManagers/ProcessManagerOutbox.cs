using NodaTime;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class ProcessManagerOutbox : IProcessManagerOutbox
    {
        private readonly List<IIdentifiedMessage<IMessage>> _messages = new();
        
        public IReadOnlyList<IIdentifiedMessage<IMessage>> Messages => _messages;
        public IReadOnlyList<IIdentifiedMessage<IScheduledMessage>> ScheduledMessages
            => _messages.OfType<IIdentifiedMessage<IScheduledMessage>>().ToList();
        public IReadOnlyList<IIdentifiedMessage<IBaseCommand>> Commands 
            => _messages.OfType<IIdentifiedMessage<IBaseCommand>>().ToList();
        public int Version { get; set; } = -1;
        
        public void Add(IMessage message, MessageMetadata metadata)
            => _messages.Add(new IdentifiedMessage<IMessage>(message, metadata));
        
        public void Add(IMessage message, MessageMetadata metadata, Instant scheduledTime)
            => _messages.Add(new IdentifiedMessage<IScheduledMessage>
            (
                new ScheduledMessage(new IdentifiedMessage<IMessage>(message, metadata), scheduledTime),
                metadata
            ));
        
        public void Clear()
        {
            Version += _messages.Count;
            _messages.Clear();
        }
    }
    
    public interface IProcessManagerOutbox
    {
        public int Version { get; }
        public IReadOnlyList<IIdentifiedMessage<IMessage>> Messages { get; }
        public IReadOnlyList<IIdentifiedMessage<IScheduledMessage>> ScheduledMessages { get; }
        public IReadOnlyList<IIdentifiedMessage<IBaseCommand>> Commands { get; }
    }
}