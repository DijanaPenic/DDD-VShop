using System;

namespace VShop.SharedKernel.EventStoreDb.Messaging
{
    public interface IMessageMetadata
    {
        public DateTime EffectiveTime { get; }
        public Guid MessageId { get; }
        public Guid CorrelationId { get; }
        public Guid CausationId { get; }
    }
}