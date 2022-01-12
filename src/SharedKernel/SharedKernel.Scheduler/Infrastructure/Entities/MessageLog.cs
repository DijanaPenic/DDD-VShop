using System;
using NodaTime;
using NodaTime.Serialization.Protobuf;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Serialization;

namespace VShop.SharedKernel.Scheduler.Infrastructure.Entities
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
        
        public MessageLog(IIdentifiedMessage<IScheduledMessage> message)
        {
            Id = message.Metadata.MessageId;
            Body = ProtobufSerializer.ToByteArray(message.Data);
            Metadata = ProtobufSerializer.ToByteArray(message.Metadata);
            Status = MessageStatus.Scheduled;
            TypeName = message.Data.TypeName;
            ScheduledTime = message.Data.ScheduledTime.ToInstant();
        }
        
        public IIdentifiedMessage<IMessage> GetMessage()
            => new IdentifiedMessage<IMessage>
            (
                ProtobufSerializer.FromByteArray(Body, ToType(TypeName)) as IMessage, 
                ProtobufSerializer.FromByteArray<MessageMetadata>(Metadata)
            );
        
        public static Type ToType(string typeName) => MessageTypeMapper.ToType(typeName);
    }
}