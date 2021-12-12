using System;
using NodaTime;
using Newtonsoft.Json;

namespace VShop.SharedKernel.Messaging
{
    public record ScheduledMessage : Message, IScheduledMessage
    {
        public string Body { get; }
        public string TypeName { get; }
        public Instant ScheduledTime { get; }

        [JsonConstructor]
        protected ScheduledMessage() { } // Needs [JsonConstructor] attribute because this constructor is protected

        public ScheduledMessage(IMessage message, Instant scheduledTime)
        {
            Body = JsonConvert.SerializeObject(message);
            TypeName = GetMessageTypeName(message.GetType());
            ScheduledTime = scheduledTime;
            CausationId = message.CausationId;
            CorrelationId = message.CorrelationId;
        }

        public static string GetMessageTypeName(Type type) => MessageTypeMapper.ToName(type);
    }
    
    public interface IScheduledMessage : IMessage
    {
        public string Body { get; }
        public string TypeName { get; }
        public Instant ScheduledTime { get; }
    }
}