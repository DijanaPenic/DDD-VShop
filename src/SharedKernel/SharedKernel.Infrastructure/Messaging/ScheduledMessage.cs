using NodaTime;
using NodaTime.Serialization.Protobuf;
using Google.Protobuf;

using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

using IMessage = VShop.SharedKernel.Infrastructure.Messaging.Contracts.IMessage;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public partial class ScheduledMessage : IScheduledMessage
    {
        public ScheduledMessage(IMessage message, Instant scheduledTime)
        {
            Body = message.ToByteString();
            TypeName = message.GetType().AssemblyQualifiedName;
            ScheduledTime = scheduledTime.ToTimestamp();
        }

        public static string ToName<T>() => typeof(T).AssemblyQualifiedName;
    }
}