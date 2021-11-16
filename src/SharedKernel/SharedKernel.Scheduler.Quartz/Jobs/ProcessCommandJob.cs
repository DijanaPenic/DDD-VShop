using Quartz;
using System;
using System.Threading.Tasks;
using Serilog;

using VShop.SharedKernel.Infrastructure.Messaging.Commands.Publishing;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.Scheduler.Quartz.Jobs
{
    public class ProcessCommandJob : IJob
    {
        public const string JobDataId = "COMMAND_ID";
        public const string JobDataBody = "COMMAND_BODY";
        
        private readonly ICommandBus _commandBus;
        
        private static readonly ILogger Logger = Log.ForContext<ProcessCommandJob>();
        
        public ProcessCommandJob(ICommandBus commandBus)
            => _commandBus = commandBus;

        public Task Execute(IJobExecutionContext context)
        {
            object command = GetCommandBody(context);
            
            Logger.Information("Sending scheduled command: {Command}", command);

            return _commandBus.SendAsync(command, context.CancellationToken);
        }

        private static object GetCommandBody(IJobExecutionContext context)
            => context.JobDetail.JobDataMap.Get(JobDataBody);
        
        private static Guid GetCommandId(IJobExecutionContext context)
            => context.JobDetail.JobDataMap.GetGuidValue(JobDataId);
    }
}