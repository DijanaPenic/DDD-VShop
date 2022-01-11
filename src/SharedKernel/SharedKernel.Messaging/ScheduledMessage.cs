using NodaTime;
using NodaTime.Serialization.Protobuf;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

using VShop.SharedKernel.Infrastructure.Serialization;

using Type = System.Type;

namespace VShop.SharedKernel.Messaging
{
    public partial class ScheduledMessage : IScheduledMessage
    {
        public ScheduledMessage(IMessage message, Instant scheduledTime)
        {
            Body = ProtobufSerializer.ToByteString(message);
            TypeName = ToName(message.GetType());
            ScheduledTime = scheduledTime.ToTimestamp();
        }
        
        public object GetMessage() => ProtobufSerializer.FromByteString(Body, ToType(TypeName));
        public static string ToName(Type type) => MessageTypeMapper.ToName(type);
        public static Type ToType(string typeName) => MessageTypeMapper.ToType(typeName);
    }

    public interface IScheduledMessage : IMessage
    {
        ByteString Body { get; }
        string TypeName { get; }
        Timestamp ScheduledTime { get; }
        object GetMessage();
    }
}