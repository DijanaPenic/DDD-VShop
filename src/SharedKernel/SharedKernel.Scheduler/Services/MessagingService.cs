using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Scheduler.Infrastructure;
using VShop.SharedKernel.Scheduler.Infrastructure.Entities;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.Scheduler.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly ICommandBus _commandBus;
        private readonly SchedulerContext _schedulerContext;
        
        private static readonly ILogger Logger = Log.ForContext<MessagingService>();

        public MessagingService(ICommandBus commandBus, SchedulerContext schedulerContext)
        {
            _commandBus = commandBus;
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
                await _commandBus.SendAsync(target, cancellationToken);
                await SetMessageStatusAsync(message, SchedulingStatus.Finished, cancellationToken);                              
            }                                                                                                                    
            catch (Exception ex)                                                                                                  
            {                                                                                                                    
                Logger.Error(ex, "Unhandled error has occurred");
                                                                                                                                 
                await SetMessageStatusAsync(message, SchedulingStatus.Failed, cancellationToken);                                
            }
        }

        private Task<MessageLog> GetScheduledMessageAsync(Guid messageId, CancellationToken cancellationToken)
            => _schedulerContext.MessageLogs
                .AsNoTracking()
                .FirstOrDefaultAsync
                (
                    m => m.Status == SchedulingStatus.Scheduled && m.Id == messageId,
                    cancellationToken
                );
        
        private async Task SetMessageStatusAsync(MessageLog message, SchedulingStatus status, CancellationToken cancellationToken)
        {
            _schedulerContext.Attach(message);
            
            // Set the new Status
            message.Status = status;

            // Mark the Status as modified, so it is the only updated value
            _schedulerContext.Entry(message).Property(m => m.Status).IsModified = true;
            
            await _schedulerContext.SaveChangesAsync(cancellationToken);
        }
    }
}