using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Events.Publishing;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Scheduler.Infrastructure;
using VShop.SharedKernel.Scheduler.Infrastructure.Entities;
using VShop.SharedKernel.Scheduler.Services.Contracts;

namespace VShop.SharedKernel.Scheduler.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly ILogger _logger;
        private readonly ICommandBus _commandBus;
        private readonly IEventBus _eventBus;
        private readonly SchedulerContext _schedulerContext;

        public MessagingService(ILogger logger, ICommandBus commandBus, IEventBus eventBus, SchedulerContext schedulerContext)
        {
            _logger = logger;
            _commandBus = commandBus;
            _eventBus = eventBus;
            _schedulerContext = schedulerContext;
        }

        public async Task SendMessageAsync(Guid messageId, CancellationToken cancellationToken)
        {
            MessageLog message = await GetScheduledMessageAsync(messageId, cancellationToken);
            await SendMessageAsync(message, cancellationToken);
        }
        
        private async Task SendMessageAsync(MessageLog message, CancellationToken cancellationToken)
        {
            object target = JsonConvert.DeserializeObject(message.Body, MessageTypeMapper.ToType(message.TypeName)); 

            try
            {
                switch (target)
                {
                    case ICommand command:
                        await _commandBus.SendAsync(command, cancellationToken);
                        break;
                    case IDomainEvent domainEvent:
                        await _eventBus.Publish(domainEvent, EventPublishStrategy.SyncStopOnException, cancellationToken);
                        break;
                    default:
                        throw new Exception("Unknown target type.");
                }

                await SetMessageStatusAsync(message, MessageStatus.Finished, cancellationToken);
            }                                                                                                                    
            catch (Exception ex)                                                                                                  
            {                                                                                                                    
                _logger.Error(ex, "Unhandled error has occurred");
                                                                                                                                 
                await SetMessageStatusAsync(message, MessageStatus.Failed, cancellationToken);                                
            }
        }

        private Task<MessageLog> GetScheduledMessageAsync(Guid messageId, CancellationToken cancellationToken)
            => _schedulerContext.MessageLogs
                .FirstOrDefaultAsync
                (
                    ml => ml.Status == MessageStatus.Scheduled && ml.Id == messageId,
                    cancellationToken
                );
        
        private async Task SetMessageStatusAsync(MessageLog message, MessageStatus status, CancellationToken cancellationToken)
        {
            message.Status = status;
            
            _schedulerContext.Entry(message).Property(ml => ml.Status).IsModified = true;
            
            await _schedulerContext.SaveChangesAsync(cancellationToken);
        }
    }
}