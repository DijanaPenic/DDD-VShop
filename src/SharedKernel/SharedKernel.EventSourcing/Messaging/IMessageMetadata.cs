using System;

namespace VShop.SharedKernel.EventSourcing.Messaging
{
    public interface IMessageMetadata
    {
        public DateTime EffectiveTime { get; }
        public Guid CorrelationId { get; }
        public Guid CausationId { get; }
    }
}