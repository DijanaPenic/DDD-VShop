using Quartz;
using Serilog;
using Newtonsoft.Json;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Commands.Publishing;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.Scheduler.Quartz.Jobs
{
    public class ProcessCommandJob : IJob
    {
        public const string JobDataBody = "COMMAND_BODY";
        public const string JobDataType = "COMMAND_TYPE";
        
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
        {
            string body = context.JobDetail.JobDataMap.GetString(JobDataBody)!;
            string typeName = context.JobDetail.JobDataMap.GetString(JobDataType)!;

            return JsonConvert.DeserializeObject(body, MessageTypeMapper.ToType(typeName));
        }
    }
}