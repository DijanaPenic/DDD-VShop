using System;

using VShop.SharedKernel.PostgresDb;

namespace VShop.SharedKernel.Scheduler.Infrastructure.Entities
{
    public class MessageLog : DbBaseEntity
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public string TypeName { get; set; }
        public DateTime ScheduledTime { get; set; }
        public SchedulingStatus Status { get; set; }
    }
}