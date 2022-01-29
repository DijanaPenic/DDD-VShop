using System;
using NodaTime;
using NodaTime.Serialization.Protobuf;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    // TODO - should this be moved to EventStoreDb project?
    // TODO - add UserId information.
    // TODO - interface?
    public partial class MessageMetadata
    {
        private MessageMetadata(Guid messageId, Guid causationId, Guid correlationId)
        {
            MessageId = messageId;
            CorrelationId = correlationId;
            CausationId = causationId;
        }
        
        public MessageMetadata(Guid messageId, Guid causationId, Guid correlationId, Instant effectiveTime)
        : this(messageId, causationId, correlationId) => EffectiveTime = effectiveTime.ToTimestamp();
    }
}