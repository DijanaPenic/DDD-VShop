using System;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public interface IMessage
    {
        string Name { get; set; }
        Guid MessageId { get; set; }
        Guid CausationId { get; set; }
        Guid CorrelationId { get; set; }
    }
}