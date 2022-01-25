using NodaTime;
using NodaTime.Serialization.Protobuf;
using Google.Protobuf;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Serialization;

using IMessage = VShop.SharedKernel.Infrastructure.Messaging.Contracts.IMessage;

namespace VShop.SharedKernel.Scheduler.DAL.Entities
{
    // TODO - rename database table.
    public class MessageLog : DbEntityBase
    {
        public Guid Id { get; }
        public byte[] Body { get; }
        public byte[] Metadata { get; }
        public string TypeName { get; }
        public Instant ScheduledTime { get; }
        public MessageStatus Status { get; set; }
        
        // For database migrations.
        public MessageLog() { } 
        
        public MessageLog(IScheduledMessage message, IMessageRegistry messageRegistry)
        {
            Id = message.Metadata.MessageId;
            Body = message.Body.ToByteArray();
            Metadata = message.Metadata.ToByteArray();
            Status = MessageStatus.Scheduled;
            TypeName = messageRegistry.GetName(Type.GetType(message.TypeName));
            ScheduledTime = message.ScheduledTime.ToInstant();
        }
        
        public IMessage GetMessage(IMessageRegistry messageRegistry)
        {
            IMessage message = (IMessage)ProtobufSerializer.FromByteArray(Body, messageRegistry.GetType(TypeName));
            message.Metadata = ProtobufSerializer.FromByteArray<MessageMetadata>(Metadata);

            return message;
        }
    }
}