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
        
        public IReadOnlyList<IMessage> Messages => _messages;
        public IReadOnlyList<IScheduledMessage> ScheduledMessages => _messages.OfType<IScheduledMessage>().ToList();
        public IReadOnlyList<IBaseCommand> Commands => _messages.OfType<IBaseCommand>().ToList();
        public int Version { get; set; } = -1;
        
        public void Add(IMessage message) => _messages.Add(message);
        public void Add(IMessage message, Instant scheduledTime) 
            => _messages.Add(new ScheduledMessage(message, scheduledTime));
        
        public void Clear()
        {
            Version += _messages.Count;
            _messages.Clear();
        }
    }
    
    public interface IProcessManagerOutbox
    {
        public int Version { get; }
        public IReadOnlyList<IMessage> Messages { get; }
        public IReadOnlyList<IScheduledMessage> ScheduledMessages { get; }
        public IReadOnlyList<IBaseCommand> Commands { get; }
    }
}