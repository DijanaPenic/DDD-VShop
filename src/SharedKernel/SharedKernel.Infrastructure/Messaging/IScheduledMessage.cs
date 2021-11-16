using System;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public interface IScheduledMessage : IMessage
    {
        public IMessage Message { get; }
        public DateTime ScheduledTime { get; }
    }
}