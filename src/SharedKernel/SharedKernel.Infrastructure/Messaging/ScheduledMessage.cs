using System;
using Newtonsoft.Json;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public record ScheduledMessage : Message, IScheduledMessage
    {
        public string Message { get; }
        
        [JsonIgnore]
        public string Type { get; }
        public DateTime ScheduledTime { get; }

        [JsonConstructor]
        protected ScheduledMessage() { }
        
        public ScheduledMessage(IMessage message, DateTime scheduledTime)
        {
            Message = JsonConvert.SerializeObject(message);
            Type = MessageTypeMapper.ToName(message.GetType());
            ScheduledTime = scheduledTime;
            CausationId = message.CausationId;
            CorrelationId = message.CorrelationId;
        }
    }
}