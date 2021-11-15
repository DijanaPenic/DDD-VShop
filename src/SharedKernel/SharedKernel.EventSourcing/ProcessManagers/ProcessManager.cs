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
        public Guid Id { get; protected set; }
        public Inbox Inbox { get; private set; } = new();
        public Outbox Outbox { get; private set; } = new();

        protected abstract void ApplyEvent(IEvent @event);

        // TODO - register command handler and execution
        protected void Register<TMessage>(Action<TMessage> handler)
            where TMessage : class, IEvent
            => Inbox.EventHandlers[typeof(TMessage)] = (message) => handler(message as TMessage);

        public void Transition(IEvent @event)
        {
            ApplyEvent(@event);
            Inbox.Events.Add(@event);
            Inbox.EventHandlers[@event.GetType()](@event);
        }

        protected void RaiseEvent(IEvent @event)
        {
            SetMessageIdentification(@event);
            Outbox.Events.Add(@event);
        }
        
        protected void RaiseCommand(ICommand command)
        {
            SetMessageIdentification(command);
            Outbox.Commands.Add(command);
        }
        
        // TODO - ScheduleCommand

        public void Load(IEnumerable<IMessage> inboxHistory, IEnumerable<IMessage> outboxHistory)
        {
            foreach (IMessage inboxMessage in inboxHistory)
            {
                if(inboxMessage is IEvent @event) ApplyEvent(@event);
                Inbox.Version++;
            }

            Outbox.Version = outboxHistory.Count() - 1;
        }
        
        public void Clear()
        {
            Outbox = default;
            Inbox = default;
        }
        
        private void SetMessageIdentification(IMessage message)
        {
            IEvent triggerEvent = Inbox.Events.Last();
            
            message.CausationId = triggerEvent.MessageId;
            message.CorrelationId = triggerEvent.CorrelationId;
        }
    }
}