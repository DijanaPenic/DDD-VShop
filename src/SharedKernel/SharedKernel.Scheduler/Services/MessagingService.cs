using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Scheduler.Database;
using VShop.SharedKernel.Scheduler.Database.Entities;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Commands.Publishing;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.Scheduler.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly ICommandBus _commandBus;
        private readonly SchedulerContext _dbContext;
        
        private static readonly ILogger Logger = Log.ForContext<MessagingService>();

        public MessagingService(ICommandBus commandBus, SchedulerContext dbContext)
        {
            _commandBus = commandBus;
            _dbContext = dbContext;
        }

        public async Task SendMessageAsync(Guid messageId, CancellationToken cancellationToken)
        {
            MessageLog message = await GetScheduledMessageAsync(messageId, cancellationToken);
            await SendMessageAsync(message, cancellationToken);
        }
        
        private async Task SendMessageAsync(MessageLog message, CancellationToken cancellationToken)
        {
            object target = JsonConvert.DeserializeObject(message.Body, MessageTypeMapper.ToType(message.RuntimeType));                                                     
                                                                                                                                 
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
            => _dbContext.MessageLogs
                .AsNoTracking()
                .FirstOrDefaultAsync
                (
                    m => m.Status == SchedulingStatus.Scheduled && m.Id == messageId,
                    cancellationToken
                );
        
        private async Task SetMessageStatusAsync(MessageLog message, SchedulingStatus status, CancellationToken cancellationToken)
        {
            _dbContext.Attach(message);
            
            // Set the new Status
            message.Status = status;

            // Mark the Status as modified, so it is the only updated value
            _dbContext.Entry(message).Property(m => m.Status).IsModified = true;
            
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}