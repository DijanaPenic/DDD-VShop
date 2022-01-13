using System;
using System.Collections.Generic;
using NodaTime;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public abstract class ProcessManager
    {
        private readonly ProcessManagerInbox _inbox = new();
        private readonly ProcessManagerOutbox _outbox = new();

        public Guid Id { get; protected set; }
        public IProcessManagerInbox Inbox => _inbox;
        public IProcessManagerOutbox Outbox => _outbox;
        public bool IsRestored => _outbox.RestoredMessages.Count > 0;

        protected abstract void ApplyEvent(IBaseEvent @event);

        protected void RegisterEvent<TEvent>(Action<TEvent, Instant> handler)
            where TEvent : class, IBaseEvent
            => _inbox.MessageHandlers[typeof(TEvent)] = 
                (message, now) => handler(message as TEvent, now);

        public void Transition(IIdentifiedEvent<IBaseEvent> @event, Instant now)
        {
            ApplyEvent(@event.Data);
            _inbox.Add(@event);

            Type eventType = @event.Data.GetType();
            
            if (_inbox.MessageHandlers.ContainsKey(eventType))
                _inbox.MessageHandlers[eventType](@event.Data, now);
            
            // else - don't need to have event handler events for all events. Some events can only trigger 
            // process manager status change.
        }

        protected void RaiseEvent(IIntegrationEvent @event) => _outbox.Add(@event);
        
        protected void RaiseCommand(IBaseCommand command) => _outbox.Add(command);
        
        protected void ScheduleCommand(IBaseCommand command, Instant scheduledTime)
            => _outbox.Add(command, scheduledTime);

        protected void ScheduleReminder(IDomainEvent @event, Instant scheduledTime)
            => _outbox.Add(@event, scheduledTime);

        public void Restore(IEnumerable<IIdentifiedMessage<IMessage>> history)
            => _outbox.Restore(history); 
        
        public void Load(IEnumerable<IIdentifiedEvent<IBaseEvent>> history)
        {
            foreach (IIdentifiedEvent<IBaseEvent> inboxMessage in history)
            {
                ApplyEvent(inboxMessage.Data);
                _inbox.Version++;
            }
        }
        
        public void Clear()
        {
            _inbox.Clear();
            _outbox.Clear();
        }
    }
}