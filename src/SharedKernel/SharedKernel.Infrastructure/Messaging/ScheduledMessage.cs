using System;
using System.Linq;
using Newtonsoft.Json;

using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Messaging.Commands;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public record ScheduledMessage : Message, IScheduledMessage
    {
        public string Body { get; }
        
        [JsonIgnore]
        public string RuntimeType { get; }
        public ScheduledMessageType MessageType { get; }
        public DateTime ScheduledTime { get; }

        [JsonConstructor]
        protected ScheduledMessage() { }
        
        public ScheduledMessage(IMessage message, DateTime scheduledTime)
        {
            Body = JsonConvert.SerializeObject(message);
            RuntimeType = MessageTypeMapper.ToName(message.GetType());
            MessageType = GetMessageType(message.GetType());
            ScheduledTime = scheduledTime;
            CausationId = message.CausationId;
            CorrelationId = message.CorrelationId;
        }

        private static ScheduledMessageType GetMessageType(Type messageType)
        {
            Type[] interfaces = messageType.GetInterfaces();

            if (interfaces.Contains(typeof(IBaseCommand)))
                return ScheduledMessageType.Command;
            
            if (interfaces.Contains(typeof(IBaseEvent)))
                return ScheduledMessageType.Event;

            throw new ArgumentException($"Unknown message type: {messageType}");
        }
    }
}