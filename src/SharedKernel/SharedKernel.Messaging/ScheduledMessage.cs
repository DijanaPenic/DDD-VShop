using NodaTime;
using NodaTime.Serialization.Protobuf;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

using VShop.SharedKernel.Infrastructure.Serialization;

using Type = System.Type;

namespace VShop.SharedKernel.Messaging
{
    public partial class ScheduledMessage : MessageContext, IScheduledMessage
    {
        public ScheduledMessage(IMessage message, Instant scheduledTime)
        {
            Body = message.ToByteString();
            TypeName = ToName(message.GetType());
            ScheduledTime = scheduledTime.ToTimestamp();
        }
        
        public IMessage GetMessage()
        {
            IMessage message = (IMessage)ProtobufSerializer.FromByteString(Body, ToType(TypeName));
            message.Metadata = Metadata;

            return message;
        }
        
        public static string ToName<T>() => ToName(typeof(T));
        public static string ToName(Type type) => MessageTypeMapper.ToName(type);
        public static Type ToType(string typeName) => MessageTypeMapper.ToType(typeName);
    }

    public interface IScheduledMessage : IMessage
    {
        ByteString Body { get; }
        string TypeName { get; }
        Timestamp ScheduledTime { get; }
        IMessage GetMessage();
    }
}