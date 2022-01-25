using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace VShop.SharedKernel.Infrastructure.Messaging.Contracts;

public interface IScheduledMessage : IMessage
{
    ByteString Body { get; }
    string TypeName { get; }
    Timestamp ScheduledTime { get; }
}