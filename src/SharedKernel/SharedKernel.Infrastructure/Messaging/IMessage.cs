using System;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public interface IMessage
    {
        Guid MessageId { get; }
        Guid CausationId { get; set; }
        Guid CorrelationId { get; set; }
    }
}