using System;

namespace VShop.SharedKernel.Scheduler.Quartz.Models
{
    public interface IScheduledCommand
    {
        public Guid Id { get; }
        public string Body { get; }
        public DateTime ScheduledTime { get; }
    }
}