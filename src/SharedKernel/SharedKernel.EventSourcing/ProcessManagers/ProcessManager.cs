using System;
using System.Linq;
using System.Collections.Generic;
using NodaTime;
using Newtonsoft.Json;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public abstract class ProcessManager
    {
        private ProcessManagerInbox _inbox = new();
        private ProcessManagerOutbox _outbox = new();

        public Guid Id { get; protected set; }
        public Guid CorrelationId { get; private set; }
        public Guid CausationId { get; private set; }
        public IProcessManagerInbox Inbox => _inbox;
        public IProcessManagerOutbox Outbox => _outbox;

        protected abstract void ApplyEvent(IBaseEvent @event);
        
        protected void RegisterEvent<TEvent>(Action<TEvent> handler)
            where TEvent : class, IBaseEvent
            => _inbox.EventHandlers[typeof(TEvent)] = (message) => handler(message as TEvent);
        
        protected void RegisterEvent<TEvent>(Action<TEvent, Instant> handler)
            where TEvent : class, IBaseEvent
            => _inbox.ScheduledEventHandlers[typeof(TEvent)] = 
                (message, now) => handler(message as TEvent, now);

        public void Transition(IBaseEvent @event, Instant now)
        {
            ApplyEvent(@event);
            _inbox.Add(@event);

            CausationId = @event.MessageId;
            CorrelationId = @event.CorrelationId;

            Type eventType = @event.GetType();
            
            if (_inbox.EventHandlers.ContainsKey(eventType)) 
                _inbox.EventHandlers[eventType](@event);
            else if (_inbox.ScheduledEventHandlers.ContainsKey(eventType))
                _inbox.ScheduledEventHandlers[eventType](@event, now);
            // else - don't need to have event handler events for all events. Some events can only trigger 
            // process manager status change.
        }

        protected void RaiseIntegrationEvent(IIntegrationEvent @event)
        {
            SetMessageIdentification(@event);
            _outbox.Add(@event);
        }

        protected void RaiseIntegrationEvent<TEvent>(string @event)
            where TEvent : IIntegrationEvent 
            => RaiseIntegrationEvent(JsonConvert.DeserializeObject<TEvent>(@event));

        protected void RaiseCommand(IBaseCommand command)
        {
            SetMessageIdentification(command);
            _outbox.Add(command);
        }
        
        protected void ScheduleCommand(IBaseCommand command, Instant scheduledTime)
        {
            SetMessageIdentification(command);
            _outbox.Add(command, scheduledTime);
        }
        
        protected void ScheduleDomainEvent(IDomainEvent @event, Instant scheduledTime)
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
            _inbox = new ProcessManagerInbox(_inbox.Version + _inbox.Count);
            _outbox = new ProcessManagerOutbox(_outbox.Version + _outbox.Count);
        }
        
        private void SetMessageIdentification(IMessage message)
        {
            message.CausationId = CausationId;
            message.CorrelationId = CorrelationId;
        }
    }
}