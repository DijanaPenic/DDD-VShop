using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Scheduler.Quartz.Services;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;

namespace VShop.SharedKernel.EventSourcing.Repositories
{
    public class ProcessManagerRepository<TProcess> : IProcessManagerRepository<TProcess>
        where TProcess : ProcessManager, new()
    {
        private readonly ILogger _logger;
        private readonly IClockService _clockService;
        private readonly EventStoreClient _eventStoreClient;
        private readonly ICommandBus _commandBus;
        private readonly ISchedulerService _messageSchedulerService;

        public ProcessManagerRepository
        (
            ILogger logger,
            IClockService clockService,
            EventStoreClient eventStoreClient,
            ICommandBus commandBus,
            ISchedulerService messageSchedulerService
        )
        {
            _logger = logger;
            _clockService = clockService;
            _eventStoreClient = eventStoreClient;
            _commandBus = commandBus;
            _messageSchedulerService = messageSchedulerService;
        }
        
        public async Task SaveAsync(TProcess processManager, CancellationToken cancellationToken)
        {
            if (processManager is null)
                throw new ArgumentNullException(nameof(processManager));

            await _eventStoreClient.AppendToStreamAsync
            (
                _clockService,
                GetInboxStreamName(processManager.Id),
                processManager.Inbox.Version,
                new[]{ processManager.Inbox.Trigger },
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

                // TODO - merge into one method
                // Schedule deferred commands
                foreach (IScheduledMessage scheduledCommand in processManager.Outbox.GetCommandsForDeferredDispatch())
                    await _messageSchedulerService.ScheduleMessageAsync(scheduledCommand, cancellationToken);

                // Schedule deferred events
                foreach (IScheduledMessage scheduledEvent in processManager.Outbox.GetEventsForDeferredDispatch())
                    await _messageSchedulerService.ScheduleMessageAsync(scheduledEvent, cancellationToken);
            }
            finally
            {
                processManager.Clear();
            }
        }
        
        public async Task<TProcess> LoadAsync(Guid processManagerId, CancellationToken cancellationToken)
        {
            IList<IMessage> inboxMessages = await _eventStoreClient.ReadStreamForwardAsync<IMessage>
            (
                GetInboxStreamName(processManagerId),
                StreamPosition.Start,
                cancellationToken
            );
            
            IList<IMessage> outboxMessages = await _eventStoreClient.ReadStreamForwardAsync<IMessage>
            (
                GetOutboxStreamName(processManagerId),
                StreamPosition.Start,
                cancellationToken
            );

            TProcess processManager = new();
            processManager.Load(inboxMessages, outboxMessages);

            return processManager;
        }

        private string GetStreamPrefix(Guid processManagerId)
        {
            string processManagerName = typeof(TProcess).Name.Replace("ProcessManager", string.Empty);
            
            return $"{_eventStoreClient.ConnectionName}/process_manager/{processManagerName}/{processManagerId}";
        }

        private string GetInboxStreamName(Guid processManagerId)
            => $"{GetStreamPrefix(processManagerId)}/inbox".ToSnakeCase();
        
        private string GetOutboxStreamName(Guid processManagerId)
            => $"{GetStreamPrefix(processManagerId)}/outbox".ToSnakeCase();
    }
}