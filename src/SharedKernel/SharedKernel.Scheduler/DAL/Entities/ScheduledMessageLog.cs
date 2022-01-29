using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.Protobuf;

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
        public string Context { get; }
        public byte[] Body { get; }
        public string TypeName { get; }
        public Instant ScheduledTime { get; }
        public MessageStatus Status { get; set; }
        
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
            Context = JsonConvert.SerializeObject(messageContext);
            Body = message.Body.ToByteArray();
            Status = MessageStatus.Scheduled;
            TypeName = messageRegistry.GetName(Type.GetType(message.TypeName));
            ScheduledTime = message.ScheduledTime.ToInstant();
        }
        
        public MessageEnvelope<IMessage> GetMessage(IMessageRegistry messageRegistry)
            => new
            (
                (IMessage)ProtobufSerializer.FromByteArray(Body, messageRegistry.GetType(TypeName)),
                JsonConvert.DeserializeObject<MessageContext>(Context)
            );

        public static string ToName<T>(IMessageRegistry messageRegistry) => messageRegistry.GetName<T>();
    }
}