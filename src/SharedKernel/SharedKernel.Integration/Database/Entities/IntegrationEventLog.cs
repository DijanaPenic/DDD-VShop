using System;
using System.Text.Json;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.PostgresDb;

namespace VShop.SharedKernel.Integration.Database.Entities
{
    public class IntegrationEventLog : DbBaseEntity
    {
        public IntegrationEventLog() { }
        public IntegrationEventLog(IIntegrationEvent @event, Guid transactionId)
        {
            EventId = @event.MessageId;
            EventTypeName = MessageTypeMapper.ToName(@event.GetType());                    
            Content = JsonSerializer.Serialize(@event);
            State = EventState.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId;
        }
        public Guid EventId { get; }
        public string EventTypeName { get; }
        public EventState State { get; set; }
        public int TimesSent { get; set; }
        public string Content { get; }
        public Guid TransactionId { get; }
    }
}