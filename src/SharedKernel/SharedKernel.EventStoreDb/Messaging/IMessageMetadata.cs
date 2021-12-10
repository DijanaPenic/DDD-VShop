using System;
using NodaTime;

namespace VShop.SharedKernel.EventStoreDb.Messaging
{
    public interface IMessageMetadata
    {
        public Instant EffectiveTime { get; }
        public Guid MessageId { get; }
        public Guid CorrelationId { get; }
        public Guid CausationId { get; }
    }
}