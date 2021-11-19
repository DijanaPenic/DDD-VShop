using System;

using VShop.SharedKernel.Infrastructure.Helpers;

namespace VShop.SharedKernel.Messaging
{
    public abstract record Message : IMessage
    {
        public Guid MessageId { get; } = SequentialGuid.Create();
        public Guid CorrelationId { get; set; }
        public Guid CausationId { get; set; }
    }
    
    public interface IMessage
    {
        Guid MessageId { get; }
        Guid CausationId { get; set; }
        Guid CorrelationId { get; set; }
    }
}