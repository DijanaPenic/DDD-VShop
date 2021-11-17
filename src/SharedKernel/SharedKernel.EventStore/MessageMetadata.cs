﻿using System;
using Newtonsoft.Json;

using VShop.SharedKernel.EventSourcing.Messaging;

// TODO - consider changing project name to EventStoreDb
namespace VShop.SharedKernel.EventStore
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