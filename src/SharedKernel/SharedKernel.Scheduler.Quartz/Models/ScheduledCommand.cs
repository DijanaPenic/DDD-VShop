using System;

namespace VShop.SharedKernel.Scheduler.Quartz.Models
{
    public record ScheduledCommand
    {
        public Guid Id { get; init; }
        public string Topic { get; init; }
        public string Body { get; init; }
        public DateTime ScheduledTime { get; init; }
    }
}