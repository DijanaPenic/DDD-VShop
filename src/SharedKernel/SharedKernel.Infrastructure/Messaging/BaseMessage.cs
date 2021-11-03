using System;
using Newtonsoft.Json;

using VShop.SharedKernel.Infrastructure.Helpers;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public abstract record BaseMessage : IMessage
    {
        [JsonIgnore]
        public Guid MessageId { get; } = SequentialGuid.Create();
        
        [JsonIgnore]
        public Guid CorrelationId { get; set; }
        
        [JsonIgnore]
        public Guid CausationId { get; set; }
    }
}