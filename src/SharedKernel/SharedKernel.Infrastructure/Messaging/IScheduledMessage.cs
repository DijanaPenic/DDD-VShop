using System;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public interface IScheduledMessage : IMessage
    {
        public string Body { get; }
        public string RuntimeType { get; }
        public DateTime ScheduledTime { get; }
    }
}