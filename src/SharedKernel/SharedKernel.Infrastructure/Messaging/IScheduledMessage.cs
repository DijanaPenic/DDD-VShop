using System;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public interface IScheduledMessage : IMessage
    {
        public string Message { get; }
        public string Type { get; }
        public DateTime ScheduledTime { get; }
    }
}