using System;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public interface IScheduledMessage : IMessage
    {
        public string Body { get; }
        public string RuntimeType { get; }
        public ScheduledMessageType MessageType { get; }
        public DateTime ScheduledTime { get; }
    }
}