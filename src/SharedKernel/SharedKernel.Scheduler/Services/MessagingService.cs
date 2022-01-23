using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Commands.Publishing.Contracts;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Events.Publishing;
using VShop.SharedKernel.Infrastructure.Events.Publishing.Contracts;
using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Scheduler.DAL.Entities;
using VShop.SharedKernel.Scheduler.Services.Contracts;

namespace VShop.SharedKernel.Scheduler.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly ILogger _logger;
        private readonly ICommandBus _commandBus;
        private readonly IEventBus _eventBus;
        private readonly SchedulerDbContext _schedulerDbContext;

        public MessagingService(ILogger logger, ICommandBus commandBus, IEventBus eventBus, SchedulerDbContext schedulerDbContext)
        {
            _logger = logger;
            _commandBus = commandBus;
            _eventBus = eventBus;
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
                        object commandResult = await _commandBus.SendAsync(command, cancellationToken);
                        if (commandResult is IResult { Value: ApplicationError error })
                            throw new Exception(error.ToString());
                        break;
                    case IBaseEvent @event:
                        await _eventBus.Publish
                        (
                            @event,
                            EventPublishStrategy.SyncStopOnException,
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