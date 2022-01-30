using System;
using NodaTime;
using NodaTime.Serialization.Protobuf;

namespace VShop.SharedKernel.EventStoreDb.Messaging
{
    public partial class MessageMetadata
    {
        private MessageMetadata(Guid messageId, Guid causationId, Guid correlationId)
        {
            MessageId = messageId;
            CorrelationId = correlationId;
            CausationId = causationId;
        }

        public MessageMetadata
        (
            Guid messageId,
            Guid causationId,
            Guid correlationId,
            Guid? userId,
            Instant effectiveTime
        ) : this(messageId, causationId, correlationId)
        {
            EffectiveTime = effectiveTime.ToTimestamp();
            UserId = userId;
        }
    }
}