using NodaTime;
using NodaTime.Serialization.Protobuf;
using Google.Protobuf;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Serialization;

using IMessage = VShop.SharedKernel.Infrastructure.Messaging.Contracts.IMessage;

namespace VShop.SharedKernel.Scheduler.DAL.Entities
{
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
        
        public MessageLog(IScheduledMessage message)
        {
            Id = message.Metadata.MessageId;
            Body = message.Body.ToByteArray();
            Metadata = message.Metadata.ToByteArray();
            Status = MessageStatus.Scheduled;
            TypeName = message.TypeName;
            ScheduledTime = message.ScheduledTime.ToInstant();
        }
        
        public IMessage GetMessage()
        {
            IMessage message = (IMessage)ProtobufSerializer.FromByteArray(Body, ToType(TypeName));
            message.Metadata = ProtobufSerializer.FromByteArray<MessageMetadata>(Metadata);

            return message;
        }
        
        public static Type ToType(string typeName) => MessageTypeMapper.ToType(typeName);
    }
}