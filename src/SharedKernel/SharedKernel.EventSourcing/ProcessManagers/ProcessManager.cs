using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;

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

            Type eventType = @event.GetType();
            if (_inbox.EventHandlers.ContainsKey(eventType)) _inbox.EventHandlers[eventType](@event);
        }
        
        public void Execute(IBaseCommand command)
        {
            _inbox.Trigger = command;
            _inbox.CommandHandlers[command.GetType()](command);
        }

        protected void RaiseIntegrationEvent(IIntegrationEvent @event)
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
        
        protected void ScheduleDomainEvent(IDomainEvent @event, DateTime scheduledTime)
        {
            SetMessageIdentification(@event);
            _outbox.Add(@event, scheduledTime);
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