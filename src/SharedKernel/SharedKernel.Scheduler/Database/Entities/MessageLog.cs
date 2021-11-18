using System;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.Scheduler.Database.Entities
{
    public class MessageLog : DbBaseEntity
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public string RuntimeType { get; set; }
        public ScheduledMessageType MessageType { get; set; }
        public DateTime ScheduledTime { get; set; }
        public SchedulingStatus Status { get; set; }
    }
}