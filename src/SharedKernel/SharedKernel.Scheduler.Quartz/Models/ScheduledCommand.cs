using System;

namespace VShop.SharedKernel.Scheduler.Quartz.Models
{
    public record ScheduledCommand : IScheduledCommand
    {
        public Guid Id { get; }
        public string Body { get; }
        public DateTime ScheduledTime { get; }

        public ScheduledCommand(Guid id, string body, DateTime scheduledTime)
        {
            Id = id;
            Body = body;
            ScheduledTime = scheduledTime;
        }
    }
}