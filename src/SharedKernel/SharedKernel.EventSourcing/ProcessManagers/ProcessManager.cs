using System;
using System.Linq;
using System.Collections.Generic;
using NodaTime;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public abstract class ProcessManager
    {
        private readonly ProcessManagerInbox _inbox = new();
        private readonly ProcessManagerOutbox _outbox = new();

        public Guid Id { get; protected set; }
        public IProcessManagerInbox Inbox => _inbox;
        public IProcessManagerOutbox Outbox => _outbox;
        public bool IsRestored { get; private set; }

        protected abstract void ApplyEvent(IBaseEvent @event);

        protected void RegisterEvent<TEvent>(Action<TEvent, Instant> handler)
            where TEvent : class, IBaseEvent
            => _inbox.MessageHandlers[typeof(TEvent)] = 
                (message, now) => handler(message as TEvent, now);

        public void Transition(IBaseEvent @event, Instant now)
        {
            ApplyEvent(@event);
            _inbox.Add(@event);

            Type eventType = @event.GetType();
            
            if (_inbox.MessageHandlers.ContainsKey(eventType))
                _inbox.MessageHandlers[eventType](@event, now);
            
            // else - don't need to have event handler events for all events. Some events can only trigger 
            // process manager status change.
        }

        protected void RaiseEvent(IIntegrationEvent @event) => _outbox.Add(@event);
        
        protected void RaiseCommand(IBaseCommand command) => _outbox.Add(command);
        
        protected void ScheduleCommand(IBaseCommand command, Instant scheduledTime)
            => _outbox.Add(command, scheduledTime);

        protected void ScheduleReminder(IDomainEvent @event, Instant scheduledTime)
            => _outbox.Add(@event, scheduledTime);

        public void Restore() => IsRestored = true;
        
        public void Load(IEnumerable<IBaseEvent> inboxHistory, IEnumerable<IMessage> outboxHistory)
        {
            foreach (IBaseEvent message in inboxHistory)
            {
                ApplyEvent(message);
                _inbox.Version++;
            }

            _outbox.Version = outboxHistory.Count() -1;
        }
        
        public void Clear()
        {
            _inbox.Clear();
            _outbox.Clear();
        }
    }
}