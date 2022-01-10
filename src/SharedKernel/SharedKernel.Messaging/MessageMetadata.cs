using System;
using NodaTime;
using NodaTime.Serialization.Protobuf;

namespace VShop.SharedKernel.Messaging
{
    public partial class MessageMetadata
    {
        public MessageMetadata(Guid messageId, Guid correlationId, Guid causationId)
        {
            MessageId = messageId;
            CorrelationId = correlationId;
            CausationId = causationId;
        }
        
        public MessageMetadata(Guid messageId, Guid correlationId, Guid causationId, Instant effectiveTime)
        : this(messageId, correlationId, causationId)
            => EffectiveTime = effectiveTime.ToTimestamp();
    }
}