using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Messaging.Commands;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    // TODO - How can I issue a reminder event?
    public abstract class ProcessManager
    {
        private IEvent _incomingEvent;
        private readonly List<IEvent> _outgoingEvents = new();
        private readonly List<ICommand> _outgoingCommands = new();
        private readonly IDictionary<Type, Action<IEvent>> _eventHandlers = new Dictionary<Type, Action<IEvent>>();
        
        public Guid Id { get; protected set; }
        public int Version { get; private set; } = -1;
    
        protected abstract void ApplyEvent(IEvent @event);

        protected void Register<TMessage>(Action<TMessage> handler)
            where TMessage : class, IEvent
            => _eventHandlers[typeof(TMessage)] = (message) => handler(message as TMessage);

        public void Transition(IEvent @event)
        {
            ApplyEvent(@event);
            _incomingEvent = @event;
            _eventHandlers[@event.GetType()](@event);
        }

        protected void RaiseEvent(IEvent @event)
        {
            SetMessageIdentification(@event);
            _outgoingEvents.Add(@event);
        }
        
        protected void RaiseCommand(ICommand command)
        {
            SetMessageIdentification(command);
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
        
        private void SetMessageIdentification(IMessage message)
        {
            message.CausationId = _incomingEvent.MessageId;
            message.CorrelationId = _incomingEvent.CorrelationId;
        }
    }
}