using System;
using System.Threading.Tasks;
using Quartz;
using Serilog;

using VShop.SharedKernel.Scheduler.Services;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.Scheduler.Quartz.Jobs
{
    public class ProcessMessageJob : IJob
    {
        public const string JobDataKey = "MESSAGE_KEY";

        private readonly IMessagingService _messagingService;
        
        private static readonly ILogger Logger = Log.ForContext<ProcessMessageJob>();
        
        public ProcessMessageJob(IMessagingService messagingService)
            => _messagingService = messagingService;

        public Task Execute(IJobExecutionContext context)
        {
            Guid messageId = GetMessageId(context);
            
            Logger.Information("Sending scheduled message: {Message}", messageId);

            return _messagingService.SendMessageAsync(messageId, context.CancellationToken);
        }

        private static Guid GetMessageId(IJobExecutionContext context)
        {
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;

            return jobDataMap.GetGuidValue(JobDataKey);
        }
    }
}