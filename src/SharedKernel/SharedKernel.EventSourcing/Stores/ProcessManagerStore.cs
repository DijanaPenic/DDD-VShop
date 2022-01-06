using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;

namespace VShop.SharedKernel.EventSourcing.Stores
{
    public class ProcessManagerStore<TProcess> : IProcessManagerStore<TProcess> where TProcess : ProcessManager
    {
        private readonly IClockService _clockService;
        private readonly EventStoreClient _eventStoreClient;
        private readonly ICommandBus _commandBus;
        private readonly ISchedulerService _messageSchedulerService;

        public ProcessManagerStore
        (
            IClockService clockService,
            EventStoreClient eventStoreClient,
            ICommandBus commandBus,
            ISchedulerService messageSchedulerService
        )
        {
            _clockService = clockService;
            _eventStoreClient = eventStoreClient;
            _commandBus = commandBus;
            _messageSchedulerService = messageSchedulerService;
        }
        
        public async Task SaveAndPublishAsync(TProcess processManager, CancellationToken cancellationToken = default)
        {
            if (processManager is null)
                throw new ArgumentNullException(nameof(processManager));

            await _eventStoreClient.AppendToStreamAsync
            (
                GetInboxStreamName(processManager.Id),
                processManager.Inbox.Version,
                processManager.Inbox.Events,
                _clockService.Now,
                cancellationToken
            );
            
            await _eventStoreClient.AppendToStreamAsync
            (
                GetOutboxStreamName(processManager.Id),
                processManager.Outbox.Version,
                processManager.Outbox.Messages,
                _clockService.Now,
                cancellationToken
            );

            try
            {
                await PublishAsync(processManager, cancellationToken);
            }
            finally
            {
                processManager.Clear();
            }
        }

        public async Task<TProcess> LoadAsync
        (
            Guid processManagerId,
            Guid causationId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<IMessage> inboxMessages = await _eventStoreClient.ReadStreamForwardAsync<IMessage>
            (
                GetInboxStreamName(processManagerId),
                cancellationToken
            );
            IReadOnlyList<IMessage> outboxMessages = await _eventStoreClient.ReadStreamForwardAsync<IMessage>
            (
                GetOutboxStreamName(processManagerId),
                cancellationToken
            );

            TProcess processManager = (TProcess)Activator.CreateInstance
            (
                typeof(TProcess),
                causationId, correlationId
            );
            
            if (processManager is null) throw new Exception("Process manager instance creation failed.");
            
            processManager.Load(inboxMessages, outboxMessages);

            return processManager;
        }

        private async Task PublishAsync(TProcess processManager, CancellationToken cancellationToken = default)
        {
            // Dispatch immediate commands
            foreach (IBaseCommand command in processManager.Outbox.Commands)
            {
                object commandResult = await _commandBus.SendAsync(command, cancellationToken);
                    
                if (commandResult is IResult { Value: ApplicationError error })
                    throw new Exception(error.ToString());
            }
                
            // Schedule deferred commands and events
            foreach (IScheduledMessage scheduledCommand in processManager.Outbox.ScheduledMessages)
                await _messageSchedulerService.ScheduleMessageAsync(scheduledCommand, cancellationToken);
        }
        
        public static string GetInboxStreamName(Guid processManagerId) => $"{GetStreamPrefix(processManagerId)}/inbox";
        
        public static string GetOutboxStreamName(Guid processManagerId) => $"{GetStreamPrefix(processManagerId)}/outbox";
        
        private static string GetStreamPrefix(Guid processManagerId)
        {
            string processManagerName = typeof(TProcess).Name.Replace("ProcessManager", string.Empty);
            
            return $"process_manager/{processManagerName}/{processManagerId}";
        }
    }
}