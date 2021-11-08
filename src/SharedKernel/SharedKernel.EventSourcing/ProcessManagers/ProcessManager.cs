using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Messaging.Commands;
using VShop.SharedKernel.EventSourcing.Messaging;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    // TODO - How can I issue a reminder event?
    public abstract class ProcessManager
    {
        private IEvent _incomingEvent;
        private readonly List<IEvent> _outgoingEvents = new();
        private readonly List<ICommand> _outgoingCommands = new();
        
        public Guid Id { get; protected set; }
        public int Version { get; private set; } = -1;
    
        protected abstract void ApplyEvent(IEvent @event);
        
        protected void ProcessEvent(IEvent @event)
        {
            ApplyEvent(@event);
            _incomingEvent = @event;
        }

        protected void RaiseEvent(IEvent @event)
        {
            SetMessage(@event, _outgoingEvents.Count);
            _outgoingEvents.Add(@event);
        }
        
        protected void RaiseCommand(ICommand command)
        {
            SetMessage(command, _outgoingCommands.Count);
            _outgoingCommands.Add(command);
        }

        public void Load(IEnumerable<IMessage> history)
        {
            foreach (IMessage message in history)
            {
                if(message is IEvent @event) ApplyEvent(@event);
                Version++;
            }
        }

        public IEnumerable<ICommand> GetOutgoingCommands()
            => _outgoingCommands;

        public IEnumerable<IDomainEvent> GetOutgoingDomainEvents()
            => _outgoingEvents.OfType<IDomainEvent>();

        public IEnumerable<IMessage> GetAllMessages()
        {
            List<IMessage> messages = new() { _incomingEvent };

            messages.AddRange(_outgoingEvents);
            messages.AddRange(_outgoingCommands);

            return messages;
        }

        public void ClearAllMessages()
        {
            _outgoingEvents.Clear();
            _outgoingCommands.Clear();
            _incomingEvent = default;
        }
        
        private void SetMessage(IMessage message, int position)
        {
            if (_incomingEvent is null) 
                throw new Exception("Cannot issue new commands or events if the event inbox is empty!");
            
            message.Name = MessageTypeMapper.ToName(message.GetType());
            message.MessageId = DeterministicGuid.Create(_incomingEvent.MessageId, $"{message.Name}-{position}");
            message.CausationId = _incomingEvent.MessageId;
            message.CorrelationId = _incomingEvent.CorrelationId;
        }
    }
}