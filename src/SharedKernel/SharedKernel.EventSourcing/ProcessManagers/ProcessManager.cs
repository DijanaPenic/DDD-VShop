using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Events;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    // TODO - How can I issue a reminder event?
    public abstract class ProcessManager
    {
        private readonly List<IMessage> _events = new();
        private readonly List<IMessage> _commands = new();
        
        public Guid Id { get; protected set; } // TODO - use value object
        public int Version { get; private set; } = -1;
    
        protected abstract void When(IMessage @event);
        
        protected void Apply(IMessage @event)
        {
            When(@event);
            _events.Add(@event);
        }
        
        protected void AddCommand(params IMessage[] commands)
            => _commands.AddRange(commands);

        public void Load(IEnumerable<IMessage> history)
        {
            foreach (IMessage message in history)
            {
                if(message is IDomainEvent or IIntegrationEvent) When(message);
                Version++;
            }
        }

        public IEnumerable<IMessage> GetCommands()
            => _commands.AsEnumerable();
        
        public IEnumerable<IMessage> GetEvents() 
            => _events.AsEnumerable();
        
        public IEnumerable<IMessage> GetAllMessages() 
            => _events.Concat(_commands.Cast<IMessage>()).AsEnumerable();

        public void ClearAllMessages()
        {
            _events.Clear();
            _commands.Clear();
        }
    }
}