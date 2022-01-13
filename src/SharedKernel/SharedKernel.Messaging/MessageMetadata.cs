using System;
using NodaTime;
using NodaTime.Serialization.Protobuf;

namespace VShop.SharedKernel.Messaging
{
    public partial class MessageMetadata
    {
        public MessageMetadata(Guid messageId, Guid causationId, Guid correlationId)
        {
            MessageId = messageId;
            CorrelationId = correlationId;
            CausationId = causationId;
        }
        
        public MessageMetadata(Guid messageId, Guid causationId, Guid correlationId, Instant effectiveTime)
        : this(messageId, causationId, correlationId) => EffectiveTime = effectiveTime.ToTimestamp();
    }
}