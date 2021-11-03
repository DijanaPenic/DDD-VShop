using System;

using VShop.SharedKernel.Infrastructure.Helpers;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public abstract record BaseMessage : IMessage
    {
        public Guid MessageId { get; } = SequentialGuid.Create();
        public Guid CorrelationId { get; set; }
        public Guid CausationId { get; set; }
    }
}