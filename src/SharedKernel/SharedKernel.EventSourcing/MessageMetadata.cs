using System;
using Newtonsoft.Json;

namespace VShop.SharedKernel.EventSourcing
{
    public record MessageMetadata
    {
        public DateTime EffectiveTime { get; init; }
                
        [JsonProperty("$correlationId")] // TODO - this is EventStore naming
        public Guid CorrelationId { get; init; }
        
        [JsonProperty("$causationId")]
        public Guid CausationId { get; init; }
    }
}