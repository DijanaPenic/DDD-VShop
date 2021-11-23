using System;
using Newtonsoft.Json;

namespace VShop.SharedKernel.Messaging
{
    public record ScheduledMessage : Message, IScheduledMessage
    {
        public string Body { get; }
        public string TypeName { get; }
        public DateTime ScheduledTime { get; }

        [JsonConstructor]
        protected ScheduledMessage()
        {
            
        }

        public ScheduledMessage(IMessage message, DateTime scheduledTime)
        {
            Body = JsonConvert.SerializeObject(message);
            TypeName = MessageTypeMapper.ToName(message.GetType());
            ScheduledTime = scheduledTime;
            CausationId = message.CausationId;
            CorrelationId = message.CorrelationId;
        }
    }
    
    public interface IScheduledMessage : IMessage
    {
        public string Body { get; }
        public string TypeName { get; }
        public DateTime ScheduledTime { get; }
    }
}