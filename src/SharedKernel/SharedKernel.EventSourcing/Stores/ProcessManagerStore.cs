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
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;

namespace VShop.SharedKernel.EventSourcing.Stores
{
    public class ProcessManagerStore<TProcess> : IProcessManagerStore<TProcess>
        where TProcess : ProcessManager, new()
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
            await AppendMessagesToStreamAsync(processManager, cancellationToken);

            try
            {
                // Dispatch immediate commands
                foreach (IBaseCommand command in processManager.Outbox.GetCommandsForImmediateDispatch())
                {
                    object commandResult = await _commandBus.SendAsync(command, cancellationToken);
                    
                    if (commandResult is IResult { Value: ApplicationError error })
                        throw new Exception(error.ToString());
                }
                
                // Schedule deferred commands and events
                foreach (IScheduledMessage scheduledCommand in processManager.Outbox.GetMessagesForDeferredDispatch())
                    await _messageSchedulerService.ScheduleMessageAsync(scheduledCommand, cancellationToken);
            }
            finally
            {
                processManager.Clear();
            }
        }
        
        public async Task SaveAsync(TProcess processManager, CancellationToken cancellationToken = default)
        {
            await AppendMessagesToStreamAsync(processManager, cancellationToken);
            processManager.Clear();
        }
        
        public async Task<TProcess> LoadAsync(Guid processManagerId, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<IMessage> inboxMessages = await LoadInboxAsync(processManagerId, cancellationToken);
            IReadOnlyList<IMessage> outboxMessages = await LoadOutboxAsync(processManagerId, cancellationToken);
            
            TProcess processManager = new();
            processManager.Load(inboxMessages, outboxMessages);

            return processManager;
        }
        
        public Task<IReadOnlyList<IMessage>> LoadInboxAsync(Guid processManagerId, CancellationToken cancellationToken = default)
            => _eventStoreClient.ReadStreamForwardAsync<IMessage>
            (
                GetInboxStreamName(processManagerId),
                StreamPosition.Start,
                cancellationToken
            );
        
        public Task<IReadOnlyList<IMessage>> LoadOutboxAsync(Guid processManagerId, CancellationToken cancellationToken = default)
            => _eventStoreClient.ReadStreamForwardAsync<IMessage>
                (
                    GetOutboxStreamName(processManagerId),
                    StreamPosition.Start,
                    cancellationToken
                );

        public async Task AppendMessagesToStreamAsync(TProcess processManager, CancellationToken cancellationToken = default)
        {
            if (processManager is null)
                throw new ArgumentNullException(nameof(processManager));

            await _eventStoreClient.AppendToStreamAsync
            (
                GetInboxStreamName(processManager.Id),
                processManager.Inbox.Version,
                processManager.Inbox.GetAllMessages(),
                _clockService.Now,
                cancellationToken
            );
            
            await _eventStoreClient.AppendToStreamAsync
            (
                GetOutboxStreamName(processManager.Id),
                processManager.Outbox.Version,
                processManager.Outbox.GetAllMessages(),
                _clockService.Now,
                cancellationToken
            );
        }
        
        private string GetInboxStreamName(Guid processManagerId)
            => $"{GetStreamPrefix(processManagerId)}/inbox".ToSnakeCase();
        
        private string GetOutboxStreamName(Guid processManagerId)
            => $"{GetStreamPrefix(processManagerId)}/outbox".ToSnakeCase();
        
        private string GetStreamPrefix(Guid processManagerId)
        {
            string processManagerName = typeof(TProcess).Name.Replace("ProcessManager", string.Empty);
            
            return $"{_eventStoreClient.ConnectionName}/process_manager/{processManagerName}/{processManagerId}";
        }
    }
}