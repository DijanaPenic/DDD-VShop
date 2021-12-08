using System;
using System.Threading.Tasks;
using Quartz;
using Serilog;

using VShop.SharedKernel.Scheduler.Services;

namespace VShop.SharedKernel.Scheduler.Quartz.Jobs
{
    public class ProcessMessageJob : IJob
    {
        public const string JobDataKey = "MESSAGE_KEY";

        private readonly ILogger _logger;
        private readonly IMessagingService _messagingService;

        public ProcessMessageJob(ILogger logger, IMessagingService messagingService)
        {
            _logger = logger;
            _messagingService = messagingService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            Guid messageId = GetMessageId(context);
            
            _logger.Information("Sending scheduled message: {Message}", messageId);

            return _messagingService.SendMessageAsync(messageId, context.CancellationToken);
        }

        private static Guid GetMessageId(IJobExecutionContext context)
        {
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;

            return jobDataMap.GetGuidValue(JobDataKey);
        }
    }
}