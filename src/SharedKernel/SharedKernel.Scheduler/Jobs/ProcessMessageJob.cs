using Quartz;
using Serilog;

using VShop.SharedKernel.Scheduler.Services.Contracts;

namespace VShop.SharedKernel.Scheduler.Jobs
{
    public class ProcessMessageJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IMessagingService _messagingService;

        public ProcessMessageJob(ILogger logger, IMessagingService messagingService)
        {
            _logger = logger;
            _messagingService = messagingService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            Guid messageId = Guid.Parse(context.JobDetail.Key.Name);
            
            _logger.Information("Sending scheduled message: {Message}", messageId);

            return _messagingService.SendMessageAsync(messageId, context.CancellationToken);
        }
    }
}