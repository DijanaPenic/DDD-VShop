using System;
using NodaTime;
using NodaTime.Serialization.Protobuf;
using VShop.SharedKernel.Infrastructure.Types;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public partial class MessageMetadata
    {
        public MessageMetadata(Guid messageId, Guid causationId, Guid correlationId)
        {
            MessageId = messageId;
            CorrelationId = correlationId;
            CausationId = causationId;
        }
        
        public MessageMetadata(Guid causationId, Guid correlationId, Instant effectiveTime)
        : this(SequentialGuid.Create(), causationId, correlationId) => EffectiveTime = effectiveTime.ToTimestamp();
    }
}