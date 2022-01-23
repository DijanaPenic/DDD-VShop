using Serilog;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Scheduler.DAL.Entities;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.SharedKernel.Scheduler.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly ILogger _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly SchedulerDbContext _schedulerDbContext;

        public MessagingService
        (
            ILogger logger,
            ICommandDispatcher commandDispatcher,
            IEventDispatcher eventDispatcher,
            SchedulerDbContext schedulerDbContext
        )
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
            _eventDispatcher = eventDispatcher;
            _schedulerDbContext = schedulerDbContext;
        }

        public async Task SendMessageAsync(Guid messageId, CancellationToken cancellationToken)
        {
            MessageLog message = await GetScheduledMessageAsync(messageId, cancellationToken);
            await SendMessageAsync(message, cancellationToken);
        }
        
        private async Task SendMessageAsync(MessageLog messageLog, CancellationToken cancellationToken)
        {
            try
            {
                switch (messageLog.GetMessage())
                {
                    case IBaseCommand command:
                        object commandResult = await _commandDispatcher.SendAsync(command, cancellationToken);
                        if (commandResult is IResult { Value: ApplicationError error })
                            throw new Exception(error.ToString());
                        break;
                    case IBaseEvent @event:
                        await _eventDispatcher.PublishAsync
                        (
                            @event,
                            NotificationDispatchStrategy.SyncStopOnException,
                            cancellationToken
                        );
                        break;
                    default:
                        throw new Exception("Unknown target type.");
                }

                await SetMessageStatusAsync(messageLog, MessageStatus.Finished, cancellationToken);
            }                                                                                                                    
            catch (Exception ex)                                                                                                  
            {                                                                                                                    
                _logger.Error(ex, "Unhandled error has occurred");
                                                                                                                                 
                await SetMessageStatusAsync(messageLog, MessageStatus.Failed, cancellationToken);                                
            }
        }

        private Task<MessageLog> GetScheduledMessageAsync(Guid messageId, CancellationToken cancellationToken)
            => _schedulerDbContext.MessageLogs
                .FirstOrDefaultAsync
                (
                    ml => ml.Status == MessageStatus.Scheduled && ml.Id == messageId,
                    cancellationToken
                );

        private async Task SetMessageStatusAsync
        (
            MessageLog messageLog,
            MessageStatus status,
            CancellationToken cancellationToken
        )
        {
            messageLog.Status = status;
            
            _schedulerDbContext.Entry(messageLog).Property(ml => ml.Status).IsModified = true;
            
            await _schedulerDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}