using System;
using NodaTime;

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
    }
}