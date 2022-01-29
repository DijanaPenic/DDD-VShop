using Google.Protobuf;
using Newtonsoft.Json;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Serialization;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.Integration.DAL.Entities
{
    public class IntegrationEventLog : DbEntity
    {
        public Guid Id { get; }
        public string TypeName { get; }
        public EventState State { get; set; }
        public int TimesSent { get; set; }
        public byte[] Body { get; }
        public string Context { get; }
        public Guid TransactionId { get; }
        
        // For database migrations.
        public IntegrationEventLog() { }

        public IntegrationEventLog
        (
            IIntegrationEvent @event,
            IMessageContext messageContext,
            Guid transactionId,
            IMessageRegistry messageRegistry
        )
        {
            Id = messageContext.MessageId;
            TypeName = messageRegistry.GetName(@event.GetType());
            Body = @event.ToByteArray();
            Context = JsonConvert.SerializeObject(messageContext);
            State = EventState.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId;
        }

        public MessageEnvelope<IIntegrationEvent> GetEvent(IMessageRegistry messageRegistry)
            => new
            (
                (IIntegrationEvent)ProtobufSerializer.FromByteArray(Body, messageRegistry.GetType(TypeName)),
                JsonConvert.DeserializeObject<MessageContext>(Context)
            );
    }
}