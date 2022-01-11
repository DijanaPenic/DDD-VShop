using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Serialization;

namespace VShop.SharedKernel.Integration.Infrastructure.Entities
{
    public class IntegrationEventLog : DbEntityBase
    {
        public Guid Id { get; }
        public string TypeName { get; }
        public EventState State { get; set; }
        public int TimesSent { get; set; }
        public Byte[] Body { get; }
        public Byte[] Metadata { get; }
        public Guid TransactionId { get; }
        
        // For database migrations.
        public IntegrationEventLog() { } 
        
        public IntegrationEventLog(IIdentifiedEvent<IBaseEvent> @event, Guid transactionId)
        {
            Id = @event.Metadata.MessageId;
            TypeName = ToName(@event.Data.GetType());
            Body = ProtobufSerializer.ToByteArray(@event.Data);
            Metadata = ProtobufSerializer.ToByteArray(@event.Metadata);
            State = EventState.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId;
        }
        
        public IIdentifiedEvent<IIntegrationEvent> GetEvent()
            => new IdentifiedEvent<IIntegrationEvent>
            (
                ProtobufSerializer.FromByteArray(Body, ToType(TypeName)) as IIntegrationEvent, 
                ProtobufSerializer.FromByteArray<MessageMetadata>(Metadata)
            );
        public static string ToName(Type type) => MessageTypeMapper.ToName(type);
        public static Type ToType(string typeName) => MessageTypeMapper.ToType(typeName);
    }
}