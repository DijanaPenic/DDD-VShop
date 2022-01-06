using System;
using NodaTime;
using Newtonsoft.Json;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.PostgresDb;

namespace VShop.SharedKernel.Scheduler.Infrastructure.Entities
{
    public class MessageLog : DbEntityBase
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public string TypeName { get; set; }
        public Instant ScheduledTime { get; set; }
        public MessageStatus Status { get; set; }
        
        public T GetMessage<T>() => (T)GetMessage();
        public object GetMessage() => JsonConvert.DeserializeObject(Body, ToType(TypeName));
        public static Type ToType(string typeName) => MessageTypeMapper.ToType(typeName);
    }
}