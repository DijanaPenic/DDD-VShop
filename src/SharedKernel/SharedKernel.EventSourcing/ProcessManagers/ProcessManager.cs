using System;
using System.Linq;
using System.Collections.Generic;
using NodaTime;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public abstract class ProcessManager
    {
        private readonly ProcessManagerInbox _inbox = new();
        private readonly ProcessManagerOutbox _outbox = new();

        public Guid Id { get; protected set; }
        public IProcessManagerInbox Inbox => _inbox;
        public IProcessManagerOutbox Outbox => _outbox;
        public Guid CausationId { get; private set; }
        public Guid CorrelationId { get; private set; }

        protected abstract void ApplyEvent(IBaseEvent @event);

        protected void RegisterEvent<TEvent>(Action<TEvent, Instant> handler)
            where TEvent : class, IBaseEvent
            => _inbox.EventHandlers[typeof(TEvent)] = 
                (message, now) => handler(message as TEvent, now);

        public void Transition(IBaseEvent @event, Instant now)
        {
            CausationId = @event.MessageId;
            CorrelationId = @event.CorrelationId;
            
            ApplyEvent(@event);
            _inbox.Add(@event);

            Type eventType = @event.GetType();
            
            if (_inbox.EventHandlers.ContainsKey(eventType))
                _inbox.EventHandlers[eventType](@event, now);
            
            // else - don't need to have event handler events for all events. Some events can only trigger 
            // process manager status change.
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
        
        protected void ScheduleCommand(IBaseCommand command, Instant scheduledTime)
        {
            SetMessageIdentification(command);
            _outbox.Add(command, scheduledTime);
        }
        
        protected void ScheduleReminder(IDomainEvent @event, Instant scheduledTime)
        {
            SetMessageIdentification(@event);
            _outbox.Add(@event, scheduledTime);
        }

        public void Load(IEnumerable<IMessage> inboxHistory, IEnumerable<IMessage> outboxHistory, Guid causationId)
        {
            foreach (IMessage inboxMessage in inboxHistory)
            {
                if(inboxMessage is IBaseEvent @event) ApplyEvent(@event);
                _inbox.Version++;
            }

            // Truncate events following the specified causationId.
            IList<IMessage> outboxHistoryList = outboxHistory.ToList()
                .RemoveRangeFollowingLast(m => m.CausationId == causationId);

            // Restore aggregate state (identified by causationId param).
            (IEnumerable<IMessage> pendingOutboxMessages, IEnumerable<IMessage> processedOutboxMessages) = 
                outboxHistoryList.Split(m => m.CausationId == causationId);
            
            _outbox.Version = processedOutboxMessages.Count() - 1;

            foreach (IMessage message in pendingOutboxMessages)
            {
                switch (message)
                {
                    case IBaseCommand command:
                        RaiseCommand(command);
                        break;
                    case IIntegrationEvent integrationEvent:
                        RaiseIntegrationEvent(integrationEvent);
                        break;
                    case IScheduledMessage scheduledMessage:
                        switch (scheduledMessage.GetMessage())
                        {
                            case ICommand command:
                                ScheduleCommand(command, scheduledMessage.ScheduledTime);
                                break;
                            case IDomainEvent domainEvent:
                                ScheduleReminder(domainEvent, scheduledMessage.ScheduledTime);
                                break;
                        }
                        break;
                }
            }
        }
        
        public void Clear()
        {
            _inbox.Clear();
            _outbox.Clear();
        }
        
        private void SetMessageIdentification(IMessage message)
        {
            message.CausationId = CausationId;
            message.CorrelationId = CorrelationId;
        }
    }
}