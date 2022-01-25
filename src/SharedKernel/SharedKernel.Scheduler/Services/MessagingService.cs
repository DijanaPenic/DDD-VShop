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
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Scheduler.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly ILogger _logger;
        private readonly IMessageRegistry _messageRegistry;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly SchedulerDbContext _schedulerDbContext;

        public MessagingService
        (
            ILogger logger,
            IMessageRegistry messageRegistry,
            ICommandDispatcher commandDispatcher,
            IEventDispatcher eventDispatcher,
            SchedulerDbContext schedulerDbContext
        )
        {
            _logger = logger;
            _messageRegistry = messageRegistry;
            _commandDispatcher = commandDispatcher;
            _eventDispatcher = eventDispatcher;
            _schedulerDbContext = schedulerDbContext;
        }

        public async Task SendMessageAsync(Guid messageId, CancellationToken cancellationToken)
        {
            ScheduledMessageLog scheduledMessage = await GetScheduledMessageAsync(messageId, cancellationToken);
            await SendMessageAsync(scheduledMessage, cancellationToken);
        }
        
        private async Task SendMessageAsync(ScheduledMessageLog scheduledMessageLog, CancellationToken cancellationToken)
        {
            try
            {
                switch (scheduledMessageLog.GetMessage(_messageRegistry))
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

                await SetMessageStatusAsync(scheduledMessageLog, MessageStatus.Finished, cancellationToken);
            }                                                                                                                    
            catch (Exception ex)                                                                                                  
            {                                                                                                                    
                _logger.Error(ex, "Unhandled error has occurred");
                                                                                                                                 
                await SetMessageStatusAsync(scheduledMessageLog, MessageStatus.Failed, cancellationToken);                                
            }
        }

        private Task<ScheduledMessageLog> GetScheduledMessageAsync(Guid messageId, CancellationToken cancellationToken)
            => _schedulerDbContext.MessageLogs
                .FirstOrDefaultAsync
                (
                    ml => ml.Status == MessageStatus.Scheduled && ml.Id == messageId,
                    cancellationToken
                );

        private async Task SetMessageStatusAsync
        (
            ScheduledMessageLog scheduledMessageLog,
            MessageStatus status,
            CancellationToken cancellationToken
        )
        {
            scheduledMessageLog.Status = status;
            
            _schedulerDbContext.Entry(scheduledMessageLog).Property(ml => ml.Status).IsModified = true;
            
            await _schedulerDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}