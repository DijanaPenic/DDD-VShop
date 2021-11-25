using System;
using Newtonsoft.Json;

namespace VShop.SharedKernel.EventStoreDb.Messaging
{
    public record MessageMetadata : IMessageMetadata
    {
        public DateTime EffectiveTime { get; init; }
        public Guid MessageId { get; init; }
                
        [JsonProperty("$correlationId")]
        public Guid CorrelationId { get; init; }
        
        [JsonProperty("$causationId")]
        public Guid CausationId { get; init; }
    }
}