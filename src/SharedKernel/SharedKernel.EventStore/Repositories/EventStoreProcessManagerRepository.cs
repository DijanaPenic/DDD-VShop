using OneOf;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.Scheduler.Quartz.Services;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.EventSourcing.ProcessManagers;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Repositories
{
    public class EventStoreProcessManagerRepository<TProcess> : IProcessManagerRepository<TProcess>
        where TProcess : ProcessManager, new()
    {
        private readonly EventStoreClient _eventStoreClient;
        private readonly ICommandBus _commandBus;
        private readonly ISchedulerService _messageSchedulerService;
        private readonly IEventBus _eventBus;
        
        private static readonly ILogger Logger = Log.ForContext<EventStoreProcessManagerRepository<TProcess>>();

        public EventStoreProcessManagerRepository
        (
            EventStoreClient eventStoreClient,
            ICommandBus commandBus,
            ISchedulerService messageSchedulerService,
            IEventBus eventBus
        )
        {
            _eventStoreClient = eventStoreClient;
            _commandBus = commandBus;
            _messageSchedulerService = messageSchedulerService;
            _eventBus = eventBus;
        }
        
        public async Task SaveAsync(TProcess processManager, CancellationToken cancellationToken)
        {
            if (processManager is null)
                throw new ArgumentNullException(nameof(processManager));

            await _eventStoreClient.AppendToStreamAsync
            (
                GetInboxStreamName(processManager.Id),
                processManager.Inbox.Version,
                new[]{ processManager.Inbox.Trigger },
                cancellationToken
            );
            
            await _eventStoreClient.AppendToStreamAsync
            (
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
                    
                    if (commandResult is IOneOf { Value: ApplicationError error })
                        throw new Exception(error.ToString());
                }

                // Queue scheduled commands
                foreach (IScheduledMessage scheduledCommand in processManager.Outbox.GetCommandsForDeferredDispatch())
                {
                    await _messageSchedulerService.ScheduleMessageAsync(scheduledCommand, cancellationToken);
                }
                
                // TODO - need to see if domain events can be created by process manager.
                // FYI - integration events are being picked up by ES subscription (integration publisher).
                // Publish domain events.
                // foreach (IDomainEvent domainEvent in processManager.GetOutgoingDomainEvents())
                //     await _eventBus.Publish(@domainEvent, PublishStrategy.SyncStopOnException);
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