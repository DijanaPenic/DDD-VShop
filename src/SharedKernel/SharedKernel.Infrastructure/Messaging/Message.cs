using System;

using VShop.SharedKernel.Infrastructure.Helpers;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    // TODO - consider creating a separate Messaging project
    public abstract record Message : IMessage
    {
        public Guid MessageId { get; } = SequentialGuid.Create();
        public Guid CorrelationId { get; set; }
        public Guid CausationId { get; set; }
    }
}