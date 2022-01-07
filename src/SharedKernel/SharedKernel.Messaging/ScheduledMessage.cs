using System;
using NodaTime;
using Newtonsoft.Json;

namespace VShop.SharedKernel.Messaging
{
    public record ScheduledMessage : Message, IScheduledMessage
    {
        public string Body { get; init; }
        public string TypeName { get; init; }
        public Instant ScheduledTime { get; init; }

        [JsonConstructor]
        protected ScheduledMessage() { }

        public ScheduledMessage(IMessage message, Instant scheduledTime)
        {
            Body = JsonConvert.SerializeObject(message);
            TypeName = ToName(message.GetType());
            ScheduledTime = scheduledTime;
            CausationId = message.CausationId;
            CorrelationId = message.CorrelationId;
        }
        
        public T GetMessage<T>() => (T)GetMessage();
        public object GetMessage() => JsonConvert.DeserializeObject(Body, ToType(TypeName));
        public static string ToName<T>() => ToName(typeof(T));
        public static string ToName(Type type) => MessageTypeMapper.ToName(type);
        public static Type ToType(string typeName) => MessageTypeMapper.ToType(typeName);
    }
    
    public interface IScheduledMessage : IMessage
    {
        public string Body { get; }
        public string TypeName { get; }
        public Instant ScheduledTime { get; }
        T GetMessage<T>();
        object GetMessage();
    }
}