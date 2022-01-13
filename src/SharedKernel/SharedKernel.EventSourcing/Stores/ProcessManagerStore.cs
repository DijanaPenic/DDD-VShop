using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;

namespace VShop.SharedKernel.EventSourcing.Stores
{
    public class ProcessManagerStore<TProcess> : IProcessManagerStore<TProcess> where TProcess : ProcessManager, new()
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
            
            IList<IdentifiedMessage<IMessage>> outboxMessages = processManager
                .Outbox.QueuedMessages
                .Select(message => new IdentifiedMessage<IMessage>
                (
                    message,
                    new MessageMetadata
                    (
                        SequentialGuid.Create(),
                        messageId,
                        correlationId,
                        _clockService.Now
                    )
                )).ToList();
            
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

        public async Task PublishAsync
        (
            IEnumerable<IIdentifiedMessage<IMessage>> messages,
            CancellationToken cancellationToken = default
        )
        {
            foreach (IIdentifiedMessage<IMessage> message in messages)
            {
                switch (message.Data)
                {
                    case IBaseCommand:
                    {
                        // TODO - test; should not work.
                        object commandResult = await _commandBus.SendAsync(message, cancellationToken);
                    
                        if (commandResult is IResult { Value: ApplicationError error })
                            throw new Exception(error.ToString());
                        break;
                    }
                    case IScheduledMessage scheduledMessage:
                        await _messageSchedulerService.ScheduleMessageAsync
                        (
                            new IdentifiedMessage<IScheduledMessage>(scheduledMessage, message.Metadata),
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
            IReadOnlyList<IIdentifiedMessage<IMessage>> outboxMessages = await _eventStoreClient
                .ReadStreamForwardAsync<IMessage>(GetOutboxStreamName(processManagerId), cancellationToken);

            TProcess processManager = new();

            IList<IIdentifiedMessage<IMessage>> processedMessages = outboxMessages
                .Where(e => e.Metadata.CausationId == messageId).ToList();
            
            if(processedMessages.Any()) processManager.Restore(processedMessages);
            else
            {
                IReadOnlyList<IdentifiedEvent<IBaseEvent>> inboxMessages = (await _eventStoreClient
                        .ReadStreamForwardAsync<IBaseEvent>(GetInboxStreamName(processManagerId), cancellationToken))
                        .Select(e => new IdentifiedEvent<IBaseEvent>(e))
                        .ToList();
                
                processManager.Load(inboxMessages);
            }

            return processManager;
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