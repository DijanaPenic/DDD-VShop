using NodaTime;
using NodaTime.Serialization.Protobuf;
using Google.Protobuf;

using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

using IMessage = VShop.SharedKernel.Infrastructure.Messaging.Contracts.IMessage;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public partial class ScheduledMessage : MessageContext, IScheduledMessage
    {
        public ScheduledMessage(IMessage message, Instant scheduledTime)
        {
            Body = message.ToByteString();
            Metadata = message.Metadata;
            TypeName = message.GetType().FullName;
            ScheduledTime = scheduledTime.ToTimestamp();
        }
    }
}