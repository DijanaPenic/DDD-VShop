using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using NodaTime;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public abstract class ProcessManager
    {
        protected readonly IClockService ClockService;
        protected ProcessManager(IClockService clockService) => ClockService = clockService;
        
        private ProcessManagerInbox _inbox = new();
        private ProcessManagerOutbox _outbox = new();

        public Guid Id { get; protected set; }
        public IProcessManagerInbox Inbox => _inbox;
        public IProcessManagerOutbox Outbox => _outbox;

        protected abstract void ApplyEvent(IBaseEvent @event);
        
        protected void RegisterEvent<TEvent>(Action<TEvent> handler)
            where TEvent : class, IBaseEvent
            => _inbox.EventHandlers[typeof(TEvent)] = (message) => handler(message as TEvent);

        public void Transition(IBaseEvent @event)
        {
            ApplyEvent(@event);
            _inbox.Trigger = @event;

            Type eventType = @event.GetType();
            if (_inbox.EventHandlers.ContainsKey(eventType)) _inbox.EventHandlers[eventType](@event);
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
            _inbox = default;
            _outbox = default;
        }
        
        private void SetMessageIdentification(IMessage message)
        {
            message.CausationId = Inbox.Trigger.MessageId;
            message.CorrelationId = Inbox.Trigger.CorrelationId;
        }
    }
}