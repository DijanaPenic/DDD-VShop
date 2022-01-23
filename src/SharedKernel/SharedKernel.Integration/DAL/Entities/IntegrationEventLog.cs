using Google.Protobuf;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Serialization;

namespace VShop.SharedKernel.Integration.DAL.Entities
{
    public class IntegrationEventLog : DbEntityBase
    {
        public Guid Id { get; }
        public string TypeName { get; }
        public EventState State { get; set; }
        public int TimesSent { get; set; }
        public byte[] Body { get; }
        public byte[] Metadata { get; }
        public Guid TransactionId { get; }
        
        // For database migrations.
        public IntegrationEventLog() { } 
        
        public IntegrationEventLog(IIntegrationEvent @event, Guid transactionId)
        {
            Id = @event.Metadata.MessageId;
            TypeName = ToName(@event.GetType());
            Body = @event.ToByteArray();
            Metadata = @event.Metadata.ToByteArray();
            State = EventState.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId;
        }

        public IIntegrationEvent GetEvent()
        {
            IIntegrationEvent integrationEvent = (IIntegrationEvent)ProtobufSerializer
                .FromByteArray(Body, ToType(TypeName));

            integrationEvent.Metadata = ProtobufSerializer.FromByteArray<MessageMetadata>(Metadata);

            return integrationEvent;
        }
        
        public static string ToName(Type type) => MessageTypeMapper.ToName(type);
        public static Type ToType(string typeName) => MessageTypeMapper.ToType(typeName);
    }
}