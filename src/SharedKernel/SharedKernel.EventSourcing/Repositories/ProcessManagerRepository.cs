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
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;

namespace VShop.SharedKernel.EventSourcing.Repositories
{
    public class ProcessManagerRepository<TProcess> : IProcessManagerRepository<TProcess>
        where TProcess : ProcessManager
    {
        private readonly IClockService _clockService;
        private readonly EventStoreClient _eventStoreClient;
        private readonly ICommandBus _commandBus;
        private readonly ISchedulerService _messageSchedulerService;

        public ProcessManagerRepository
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
        
        public async Task SaveAsync(TProcess processManager, CancellationToken cancellationToken = default)
        {
            if (processManager is null)
                throw new ArgumentNullException(nameof(processManager));

            if (processManager.Inbox.Trigger is null) return;

            await _eventStoreClient.AppendToStreamAsync
            (
                _clockService,
                GetInboxStreamName(processManager.Id),
                processManager.Inbox.Version,
                new[]{ processManager.Inbox.Trigger }, // TODO - potentially create method for a single element
                cancellationToken
            );
            
            await _eventStoreClient.AppendToStreamAsync
            (
                _clockService,
                GetOutboxStreamName(processManager.Id),
                processManager.Outbox.Version,
                processManager.Outbox.GetAllMessages(),
                cancellationToken
            );

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
        
        public async Task<TProcess> LoadAsync(Guid processManagerId, CancellationToken cancellationToken = default)
        {
            IList<IMessage> inboxMessages = await LoadInboxAsync(processManagerId, cancellationToken);
            IList<IMessage> outboxMessages = await LoadOutboxAsync(processManagerId, cancellationToken);

            // TODO - think about this - code smell!!
            TProcess processManager = (TProcess)Activator.CreateInstance(typeof(TProcess), _clockService);
            if (processManager is null)
                throw new Exception($"Couldn't resolve {nameof(processManager)} instance.");
            
            processManager.Load(inboxMessages, outboxMessages);

            return processManager;
        }
        
        public Task<IList<IMessage>> LoadInboxAsync(Guid processManagerId, CancellationToken cancellationToken = default)
            => _eventStoreClient.ReadStreamForwardAsync<IMessage>
            (
                GetInboxStreamName(processManagerId),
                StreamPosition.Start,
                cancellationToken
            );
        
        public Task<IList<IMessage>> LoadOutboxAsync(Guid processManagerId, CancellationToken cancellationToken = default)
            => _eventStoreClient.ReadStreamForwardAsync<IMessage>
                (
                    GetOutboxStreamName(processManagerId),
                    StreamPosition.Start,
                    cancellationToken
                );

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