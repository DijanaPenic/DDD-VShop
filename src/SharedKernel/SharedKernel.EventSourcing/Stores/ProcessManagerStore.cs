﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Scheduler.Services.Contracts;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.EventSourcing.Stores
{
    public class ProcessManagerStore<TProcess> : IProcessManagerStore<TProcess> where TProcess : ProcessManager, new()
    {
        private readonly CustomEventStoreClient _eventStoreClient;
        private readonly IMessageContextRegistry _messageContextRegistry;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ISchedulerService _messageSchedulerService;
        private readonly IContext _context;

        public ProcessManagerStore
        (
            CustomEventStoreClient eventStoreClient,
            IMessageContextRegistry messageContextRegistry,
            ICommandDispatcher commandDispatcher,
            ISchedulerService messageSchedulerService,
            IContext context
        )
        {
            _eventStoreClient = eventStoreClient;
            _messageContextRegistry = messageContextRegistry;
            _commandDispatcher = commandDispatcher;
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
            
            foreach (IMessage message in processManager.Inbox.Events.Concat(processManager.Outbox.Messages))
                _messageContextRegistry.Set(message, new MessageContext(_context));

            await _eventStoreClient.AppendToStreamAsync
            (
                GetInboxStreamName(processManager.Id),
                processManager.Inbox.Version,
                processManager.Inbox.Events,
                cancellationToken
            );

            await _eventStoreClient.AppendToStreamAsync
            (
                GetOutboxStreamName(processManager.Id),
                processManager.Outbox.Version,
                processManager.Outbox.Messages,
                cancellationToken
            );

            try
            {
                await PublishAsync(processManager.Outbox.Messages, cancellationToken);
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
            
            foreach ((IMessage message, IMessageContext messageContext) in processed)
                _messageContextRegistry.Set(message, messageContext);
            
            await PublishAsync(processed.ToMessages(), cancellationToken); 
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