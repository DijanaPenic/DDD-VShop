﻿using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging;

namespace VShop.SharedKernel.Scheduler.Quartz.Services
{
    public interface ISchedulerService
    {
        Task ScheduleMessageAsync(IScheduledMessage message, CancellationToken cancellationToken = default);
    }
}