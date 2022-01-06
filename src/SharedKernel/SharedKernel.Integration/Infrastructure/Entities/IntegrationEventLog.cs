using System;
using Newtonsoft.Json;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.PostgresDb;

namespace VShop.SharedKernel.Integration.Infrastructure.Entities
{
    public class IntegrationEventLog : DbEntityBase
    {
        public Guid EventId { get; } // TODO - renaming
        public string EventTypeName { get; }
        public EventState State { get; set; }
        public int TimesSent { get; set; }
        public string Content { get; }
        public Guid TransactionId { get; }
        
        public IntegrationEventLog() { } // Needed for database migrations.
        public IntegrationEventLog(IIntegrationEvent @event, Guid transactionId)
        {
            EventId = @event.MessageId;
            EventTypeName = ToName(@event.GetType());
            Content = JsonConvert.SerializeObject(@event);
            State = EventState.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId;
        }
        
        public T GetEvent<T>() => (T)GetEvent();
        public object GetEvent() => JsonConvert.DeserializeObject(Content, ToType(EventTypeName));
        public static string ToName<T>() => ToName(typeof(T));
        public static string ToName(Type type) => MessageTypeMapper.ToName(type);
        public static Type ToType(string typeName) => MessageTypeMapper.ToType(typeName);
    }
}