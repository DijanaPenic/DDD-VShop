using NodaTime;
using NodaTime.Serialization.Protobuf;
    
using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Serialization;

using IMessage = VShop.SharedKernel.Infrastructure.Messaging.Contracts.IMessage;

namespace VShop.SharedKernel.Scheduler.DAL.Entities
{
    public class ScheduledMessageLog : DbEntity
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public Guid CausationId { get; }
        public Guid CorrelationId { get; }
        public byte[] Body { get; }
        public string TypeName { get; }
        public Instant ScheduledTime { get; }
        public ScheduledMessageStatus Status { get; set; }
        
        // For database migrations.
        public ScheduledMessageLog() { }

        public ScheduledMessageLog
        (
            IScheduledMessage message,
            IMessageContext messageContext,
            IMessageRegistry messageRegistry
        )
        {
            Id = messageContext.MessageId;
            UserId = messageContext.Context.Identity.Id;
            CausationId = messageContext.Context.RequestId;
            CorrelationId = messageContext.Context.CorrelationId;
            Body = message.Body.ToByteArray();
            Status = ScheduledMessageStatus.Scheduled;
            TypeName = messageRegistry.GetName(Type.GetType(message.TypeName));
            ScheduledTime = message.ScheduledTime.ToInstant();
        }
        
        public MessageEnvelope<IMessage> GetMessage(IMessageRegistry messageRegistry)
            => new
            (
                (IMessage)ProtobufSerializer.FromByteArray(Body, messageRegistry.GetType(TypeName)),
                new MessageContext(Id, new Context(CausationId, CorrelationId, new IdentityContext(UserId)))
            );

        public static string ToName<T>(IMessageRegistry messageRegistry) => messageRegistry.GetName<T>();
    }
}