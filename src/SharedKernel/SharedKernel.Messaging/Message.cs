using System;

using VShop.SharedKernel.Infrastructure.Helpers;

namespace VShop.SharedKernel.Messaging
{
    // TODO - refactoring needed: client control
    public abstract record Message : IMessage
    {
        public Guid MessageId { get; set; } = SequentialGuid.Create();
        public Guid CorrelationId { get; set; }
        public Guid CausationId { get; set; }
    }
    
    public interface IMessage
    {
        Guid MessageId { get; set; }
        Guid CausationId { get; set; }
        Guid CorrelationId { get; set; }
    }
}