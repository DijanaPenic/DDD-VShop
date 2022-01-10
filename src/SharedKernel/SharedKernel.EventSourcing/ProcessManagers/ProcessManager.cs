using System;
using System.Linq;
using System.Collections.Generic;
using NodaTime;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Infrastructure.Helpers;
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
            => _inbox.MessageHandlers[typeof(TEvent)] = 
                (message, now) => handler(message as TEvent, now);

        public void Transition(IIdentifiedMessage<IBaseEvent> @event, Instant now)
        {
            CausationId = @event.Metadata.MessageId;
            CorrelationId = @event.Metadata.CorrelationId;
            
            ApplyEvent(@event.Data);
            _inbox.Add(@event);

            Type eventType = @event.GetType();
            
            if (_inbox.MessageHandlers.ContainsKey(eventType))
                _inbox.MessageHandlers[eventType](@event, now);
            
            // else - don't need to have event handler events for all events. Some events can only trigger 
            // process manager status change.
        }

        protected void RaiseIntegrationEvent(IIntegrationEvent @event)
            => _outbox.Add(new IdentifiedMessage<IMessage>(@event, GetMetadata()));

        protected void RaiseCommand(IBaseCommand command)
            => _outbox.Add(new IdentifiedMessage<IMessage>(command, GetMetadata()));
        
        protected void ScheduleCommand(IBaseCommand command, Instant scheduledTime)
            => _outbox.Add
            (
                new IdentifiedMessage<IMessage>(command, GetMetadata()),
                scheduledTime
            );
        
        protected void ScheduleReminder(IDomainEvent @event, Instant scheduledTime)
            => _outbox.Add
            (
                new IdentifiedMessage<IMessage>(@event, GetMetadata()),
                scheduledTime
            );

        public void Load
        (
            IEnumerable<IIdentifiedMessage<IMessage>> inboxHistory,
            IEnumerable<IIdentifiedMessage<IMessage>> outboxHistory,
            Guid causationId,
            Guid correlationId
        )
        {
            CausationId = causationId;
            CorrelationId = correlationId;
            
            foreach (IIdentifiedMessage<IMessage> inboxMessage in inboxHistory)
            {
                if(inboxMessage.Data is IBaseEvent @event) ApplyEvent(@event);
                _inbox.Version++;
            }

            // Truncate events following the specified causationId.
            IList<IIdentifiedMessage<IMessage>> outboxHistoryList = outboxHistory.ToList()
                .RemoveRangeFollowingLast(m => m.Metadata.CausationId == CausationId);

            // Restore aggregate state (identified by causationId param).
            (IEnumerable<IIdentifiedMessage<IMessage>> pendingOutboxMessages, IEnumerable<IIdentifiedMessage<IMessage>> processedOutboxMessages) = 
                outboxHistoryList.Split(m => m.Metadata.CausationId == CausationId);
            
            _outbox.Version = processedOutboxMessages.Count() - 1;

            foreach (IIdentifiedMessage<IMessage> message in pendingOutboxMessages)
            {
                switch (message.Data)
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
                            case IBaseCommand command:
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
        
        private MessageMetadata GetMetadata() => new(SequentialGuid.Create(), CorrelationId, CausationId);
    }
}