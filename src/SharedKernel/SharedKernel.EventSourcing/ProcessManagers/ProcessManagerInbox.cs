using System;
using System.Collections.Generic;
using NodaTime;

using VShop.SharedKernel.Messaging;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public class ProcessManagerInbox : IProcessManagerInbox
    {
        private readonly List<IIdentifiedMessage<IMessage>> _messages = new();
        
        public IReadOnlyList<IIdentifiedMessage<IMessage>> Messages => _messages;
        public int Version { get; set; } = -1;
        public IDictionary<Type, Action<IIdentifiedMessage<IMessage>, Instant>> MessageHandlers { get; }
            = new Dictionary<Type, Action<IIdentifiedMessage<IMessage>, Instant>>();

        public void Add(IIdentifiedMessage<IMessage> @event) => _messages.Add(@event);
        public void Clear()
        {
            Version += _messages.Count;
            _messages.Clear();
        }
    }
    
    public interface IProcessManagerInbox
    {
        public int Version { get; }
        IReadOnlyList<IIdentifiedMessage<IMessage>> Messages { get; }
    }
}