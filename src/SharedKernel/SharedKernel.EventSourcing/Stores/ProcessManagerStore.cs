using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;

namespace VShop.SharedKernel.EventSourcing.Stores
{
    public class ProcessManagerStore<TProcess> : IProcessManagerStore<TProcess> where TProcess : ProcessManager, new()
    {
        private readonly CustomEventStoreClient _eventStoreClient;
        private readonly IMessageContextRegistry _messageContextRegistry;
        private readonly IModuleClient _moduleClient;
        private readonly ISchedulerService _messageSchedulerService;
        private readonly IContext _context;

        public ProcessManagerStore
        (
            CustomEventStoreClient eventStoreClient,
            IMessageContextRegistry messageContextRegistry,
            IModuleClient moduleClient,
            ISchedulerService messageSchedulerService,
            IContext context
        )
        {
            _eventStoreClient = eventStoreClient;
            _messageContextRegistry = messageContextRegistry;
            _moduleClient = moduleClient;
            _messageSchedulerService = messageSchedulerService;
            _context = context;
        }
        
        public async Task SaveAndPublishAsync
        (
            TProcess processManager,
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

            IList<MessageEnvelope<IMessage>> outboxMessages = processManager.Outbox.Messages
                .Select(m => new MessageEnvelope<IMessage>(m, new MessageContext(_context)))
                .ToList();
            
            _messageContextRegistry.Set(outboxMessages);

            await _eventStoreClient.AppendToStreamAsync
            (
                GetOutboxStreamName(processManager.Id),
                processManager.Outbox.Version,
                processManager.Outbox.Messages,
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

        private async Task PublishAsync
        (
            IEnumerable<MessageEnvelope<IMessage>> messages,
            CancellationToken cancellationToken = default
        )
        {
            foreach ((IMessage message, IMessageContext messageContext) in messages)
            {
                switch (message)
                {
                    case IBaseCommand command:
                    {
                        await _moduleClient.PublishAsync(command, cancellationToken);
                        break;
                    }
                    case IScheduledMessage scheduledMessage:
                    {
                        await _messageSchedulerService.ScheduleMessageAsync
                        (
                            new MessageEnvelope<IScheduledMessage>(scheduledMessage, messageContext),
                            cancellationToken
                        );
                        break;
                    }
                }
            }
        }

        public async Task<TProcess> LoadAsync
        (
            Guid processManagerId,
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<MessageEnvelope<IBaseEvent>> inboxEnvelopes = await _eventStoreClient
                .ReadStreamForwardAsync<IBaseEvent>(GetInboxStreamName(processManagerId), cancellationToken);
            
            IReadOnlyList<MessageEnvelope<IMessage>> outboxEnvelopes = await _eventStoreClient
                .ReadStreamForwardAsync<IMessage>(GetOutboxStreamName(processManagerId), cancellationToken);

            TProcess processManager = new();
            processManager.Load(inboxEnvelopes.ToMessages(), outboxEnvelopes.ToMessages());

            IList<MessageEnvelope<IMessage>> processed = outboxEnvelopes
                .Where(e => e.MessageContext.Context.RequestId == _context.RequestId).ToList();

            if (!processed.Any()) return processManager;

            _messageContextRegistry.Set(processed);
            
            await PublishAsync(processed, cancellationToken); 
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
            return $"{processManagerName}/{processManagerId}";
        }
    }
}