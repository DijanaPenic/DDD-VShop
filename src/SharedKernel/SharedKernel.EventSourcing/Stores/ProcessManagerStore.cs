using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.EventSourcing.Stores
{
    public class ProcessManagerStore<TProcess> : IProcessManagerStore<TProcess> where TProcess : ProcessManager, new()
    {
        private readonly IClockService _clockService;
        private readonly CustomEventStoreClient _eventStoreClient;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ISchedulerService _messageSchedulerService;

        public ProcessManagerStore
        (
            IClockService clockService,
            CustomEventStoreClient eventStoreClient,
            ICommandDispatcher commandDispatcher,
            ISchedulerService messageSchedulerService
        )
        {
            _clockService = clockService;
            _eventStoreClient = eventStoreClient;
            _commandDispatcher = commandDispatcher;
            _messageSchedulerService = messageSchedulerService;
        }
        
        public async Task SaveAndPublishAsync
        (
            TProcess processManager,
            Guid messageId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        )
        {
            if (processManager is null)
                throw new ArgumentNullException(nameof(processManager));

            await _eventStoreClient.AppendToStreamAsync
            (
                GetInboxStreamName(processManager.Id),
                processManager.Inbox.Version,
                processManager.Inbox.Events,
                cancellationToken
            );
            
            IList<IMessage> outboxMessages = processManager.Outbox.Messages.Select(message =>
            {
                message.Metadata = new MessageMetadata
                (
                    SequentialGuid.Create(),
                    messageId,
                    correlationId,
                    _clockService.Now
                );
                return message;
            }).ToList();
            
            await _eventStoreClient.AppendToStreamAsync
            (
                GetOutboxStreamName(processManager.Id),
                processManager.Outbox.Version,
                outboxMessages,
                cancellationToken
            );

            try
            {
                await PublishAsync(outboxMessages, cancellationToken);
            }
            finally
            {
                processManager.Clear();
            }
        }

        public async Task PublishAsync(IEnumerable<IMessage> messages, CancellationToken cancellationToken = default)
        {
            foreach (IMessage message in messages)
            {
                switch (message)
                {
                    case IBaseCommand command:
                    {
                        object commandResult = await _commandDispatcher.SendAsync(command, cancellationToken);
                    
                        if (commandResult is IResult { Value: ApplicationError error })
                            throw new Exception(error.ToString());
                        break;
                    }
                    case IScheduledMessage scheduledMessage:
                        await _messageSchedulerService.ScheduleMessageAsync
                        (
                            scheduledMessage,
                            cancellationToken
                        );
                        break;
                }
            }
        }

        public async Task<TProcess> LoadAsync
        (
            Guid processManagerId,
            Guid messageId,
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<IBaseEvent> inboxMessages = await _eventStoreClient
                .ReadStreamForwardAsync<IBaseEvent>(GetInboxStreamName(processManagerId), cancellationToken);
            
            IReadOnlyList<IMessage> outboxMessages = await _eventStoreClient
                .ReadStreamForwardAsync<IMessage>(GetOutboxStreamName(processManagerId), cancellationToken);

            TProcess processManager = new();
            processManager.Load(inboxMessages, outboxMessages);
            
            IList<IMessage> processedMessages = outboxMessages
                .Where(e => e.Metadata.CausationId == messageId).ToList();

            if (!processedMessages.Any()) return processManager;
            
            await PublishAsync(processedMessages, cancellationToken); 
            processManager.Restore();

            return processManager;
        }
        
        public static string GetInboxStreamName(Guid processManagerId) 
            => $"{GetStreamPrefix(processManagerId)}/inbox";
        
        public static string GetOutboxStreamName(Guid processManagerId) 
            => $"{GetStreamPrefix(processManagerId)}/outbox";
        
        private static string GetStreamPrefix(Guid processManagerId)
        {
            string processManagerName = typeof(TProcess).Name.Replace("ProcessManager", string.Empty);
            
            return $"process_manager/{processManagerName}/{processManagerId}";
        }
    }
}