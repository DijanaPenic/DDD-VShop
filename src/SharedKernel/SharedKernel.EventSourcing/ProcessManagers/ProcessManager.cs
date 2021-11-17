using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Messaging.Commands;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public abstract class ProcessManager
    {
        private Inbox _inbox = new();
        private Outbox _outbox = new();

        public Guid Id { get; protected set; }
        public IInbox Inbox => _inbox;
        public IOutbox Outbox => _outbox;

        protected abstract void ApplyEvent(IBaseEvent @event);
        
        protected void RegisterEvent<TEvent>(Action<TEvent> handler)
            where TEvent : class, IBaseEvent
            => _inbox.EventHandlers[typeof(TEvent)] = (message) => handler(message as TEvent);
        
        protected void RegisterCommand<TCommand>(Action<TCommand> handler)
            where TCommand : class, IBaseCommand
            => _inbox.CommandHandlers[typeof(TCommand)] = (message) => handler(message as TCommand);

        public void Transition(IBaseEvent @event)
        {
            ApplyEvent(@event);
            _inbox.Trigger = @event;
            _inbox.EventHandlers[@event.GetType()](@event);
        }
        
        public void Execute(IBaseCommand command)
        {
            _inbox.Trigger = command;
            _inbox.CommandHandlers[command.GetType()](command);
        }

        protected void RaiseEvent(IBaseEvent @event)
        {
            SetMessageIdentification(@event);
            _outbox.Add(@event);
        }
        
        protected void RaiseCommand(IBaseCommand command)
        {
            SetMessageIdentification(command);
            _outbox.Add(command);
        }
        
        protected void ScheduleCommand(IBaseCommand command, DateTime scheduledTime)
        {
            SetMessageIdentification(command);
            _outbox.Add(command, scheduledTime);
        }

        public void Load(IEnumerable<IMessage> inboxHistory, IEnumerable<IMessage> outboxHistory)
        {
            foreach (IMessage inboxMessage in inboxHistory)
            {
                if(inboxMessage is IBaseEvent @event) ApplyEvent(@event);
                _inbox.Version++;
            }

            _outbox.Version = outboxHistory.Count() - 1;
        }
        
        public void Clear()
        {
            _outbox = default;
            _inbox = default;
        }
        
        private void SetMessageIdentification(IMessage message)
        {
            message.CausationId = Inbox.Trigger.MessageId;
            message.CorrelationId = Inbox.Trigger.CorrelationId;
        }
    }
}