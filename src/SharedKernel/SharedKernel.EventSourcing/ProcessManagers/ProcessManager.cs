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
        private readonly List<IMessage> _incomingEvents = new();
        private readonly List<IMessage> _outgoingEvents = new();
        private readonly List<IMessage> _outgoingCommands = new();
        
        public Guid Id { get; protected set; }
        public int Version { get; private set; } = -1;
    
        protected abstract void Apply(IMessage @event);
        
        protected void ProcessEvent(IMessage @event)
        {
            Apply(@event);
            _incomingEvents.Add(@event);
        }
        
        protected void RaiseCommand(params IMessage[] commands)
            => _outgoingCommands.AddRange(commands);
        
        protected void RaiseEvent(params IMessage[] events)
            => _outgoingEvents.AddRange(events);

        public void Load(IEnumerable<IMessage> history)
        {
            foreach (IMessage message in history)
            {
                if(message is IDomainEvent or IIntegrationEvent) Apply(message);
                Version++;
            }
        }

        public IEnumerable<IMessage> GetOutgoingCommands()
            => _outgoingCommands;
        
        public IEnumerable<IDomainEvent> GetOutgoingDomainEvents()
            => _outgoingEvents
                .Where(e => e is IDomainEvent)
                .Cast<IDomainEvent>();
        
        public IEnumerable<IMessage> GetAllMessages()
            => _incomingEvents
                .Concat(_outgoingCommands)
                .Concat(_outgoingEvents);

        public void ClearAllMessages()
        {
            _incomingEvents.Clear();
            _outgoingEvents.Clear();
            _outgoingCommands.Clear();
        }
    }
}