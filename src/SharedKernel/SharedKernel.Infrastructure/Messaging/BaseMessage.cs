using System;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public abstract record BaseMessage : IMessage
    {
        public string Name { get; set; }
        public Guid MessageId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid CausationId { get; set; }
    }
}