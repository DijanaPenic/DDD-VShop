using Serilog;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Scheduler.DAL;
using VShop.SharedKernel.Scheduler.DAL.Entities;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;

namespace VShop.SharedKernel.Scheduler.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly ILogger _logger;
        private readonly IMessageRegistry _messageRegistry;
        private readonly IMessageContextRegistry _messageContextRegistry;
        private readonly IModuleClient _moduleClient;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly SchedulerDbContext _schedulerDbContext;

        public MessagingService
        (
            ILogger logger,
            IMessageRegistry messageRegistry,
            IMessageContextRegistry messageContextRegistry,
            IModuleClient moduleClient,
            IEventDispatcher eventDispatcher,
            SchedulerDbContext schedulerDbContext
        )
        {
            _logger = logger;
            _messageRegistry = messageRegistry;
            _messageContextRegistry = messageContextRegistry;
            _moduleClient = moduleClient;
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
                (IMessage message, IMessageContext messageContext) = scheduledMessageLog.GetMessage(_messageRegistry);
                _messageContextRegistry.Set(message, messageContext);
                
                switch (message)
                {
                    case IBaseCommand command:
                        await _moduleClient.PublishAsync(command, cancellationToken);
                        break;
                    case IBaseEvent @event:
                        await _eventDispatcher.PublishAsync(@event, cancellationToken);
                        break;
                    default:
                        throw new Exception("Unknown target type.");
                }

                await SetMessageStatusAsync(scheduledMessageLog, ScheduledMessageStatus.Finished, cancellationToken);
            }                                                                                                                    
            catch (Exception ex)                                                                                                  
            {                                                                                                                    
                _logger.Error(ex, "Unhandled error has occurred");
                                                                                                                                 
                await SetMessageStatusAsync(scheduledMessageLog, ScheduledMessageStatus.Failed, cancellationToken);                                
            }
        }

        private Task<ScheduledMessageLog> GetScheduledMessageAsync(Guid messageId, CancellationToken cancellationToken)
            => _schedulerDbContext.MessageLogs
                .FirstOrDefaultAsync
                (
                    ml => ml.Status == ScheduledMessageStatus.Scheduled && ml.Id == messageId,
                    cancellationToken
                );

        private async Task SetMessageStatusAsync
        (
            ScheduledMessageLog scheduledMessageLog,
            ScheduledMessageStatus status,
            CancellationToken cancellationToken
        )
        {
            scheduledMessageLog.Status = status;
            
            _schedulerDbContext.Entry(scheduledMessageLog).Property(ml => ml.Status).IsModified = true;
            
            await _schedulerDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}