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
        protected ScheduledMessage(string body, string typeName, Instant scheduledTime)
        {
            Body = body;
            TypeName = typeName;
            ScheduledTime = scheduledTime;
        }

        public ScheduledMessage(IMessage message, Instant scheduledTime)
        {
            Body = JsonConvert.SerializeObject(message);
            TypeName = MessageTypeMapper.ToName(message.GetType());
            ScheduledTime = scheduledTime;
            CausationId = message.CausationId;
            CorrelationId = message.CorrelationId;
        }
        
        public object GetMessage() => JsonConvert.DeserializeObject(Body, MessageTypeMapper.ToType(TypeName));  
    }
    
    public interface IScheduledMessage : IMessage
    {
        public string Body { get; }
        public string TypeName { get; }
        public Instant ScheduledTime { get; }
        object GetMessage();
    }
}