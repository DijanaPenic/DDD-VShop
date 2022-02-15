using Google.Protobuf;

using VShop.SharedKernel.Infrastructure.Contexts;
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
        public Guid? UserId { get; }
        public Guid CausationId { get; }
        public Guid CorrelationId { get; }
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
            UserId = messageContext.Context.Identity.UserId;
            CausationId = messageContext.Context.RequestId;
            CorrelationId = messageContext.Context.CorrelationId;
            TypeName = messageRegistry.GetName(@event.GetType());
            Body = @event.ToByteArray();
            State = EventState.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId;
        }

        public MessageEnvelope<IIntegrationEvent> GetEvent(IMessageRegistry messageRegistry)
            => new
            (
                (IIntegrationEvent)ProtobufSerializer.FromByteArray(Body, messageRegistry.GetType(TypeName)),
                new MessageContext(Id, new Context(CausationId, CorrelationId, new IdentityContext(UserId)))
            );
    }
}