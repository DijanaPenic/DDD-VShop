using System;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public record ScheduledMessage : Message, IScheduledMessage
    {
        public IMessage Message { get; }
        public DateTime ScheduledTime { get; }

        public ScheduledMessage(IMessage message, DateTime scheduledTime)
        {
            Message = message;
            ScheduledTime = scheduledTime;
            CausationId = message.CausationId;
            CorrelationId = message.CorrelationId;
        }
    }
}