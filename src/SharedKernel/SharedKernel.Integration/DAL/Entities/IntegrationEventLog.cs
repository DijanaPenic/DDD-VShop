using Google.Protobuf;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Serialization;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

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
        
        public IntegrationEventLog(IIntegrationEvent @event, Guid transactionId, IMessageRegistry messageRegistry)
        {
            Id = @event.Metadata.MessageId;
            TypeName = messageRegistry.GetName(@event.GetType());
            Body = @event.ToByteArray();
            Metadata = @event.Metadata.ToByteArray();
            State = EventState.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId;
        }

        public IIntegrationEvent GetEvent(IMessageRegistry messageRegistry)
        {
            IIntegrationEvent integrationEvent = (IIntegrationEvent)ProtobufSerializer
                .FromByteArray(Body, messageRegistry.GetType(TypeName));

            integrationEvent.Metadata = ProtobufSerializer.FromByteArray<MessageMetadata>(Metadata);

            return integrationEvent;
        }
    }
}