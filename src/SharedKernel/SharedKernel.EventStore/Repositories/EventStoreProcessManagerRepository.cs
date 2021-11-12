using OneOf;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using EventStore.Client;

using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Commands;
using VShop.SharedKernel.Infrastructure.Messaging.Events.Publishing;
using VShop.SharedKernel.Infrastructure.Messaging.Commands.Publishing;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.EventStore.Extensions;
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
        private readonly Publisher _publisher;
        
        private static readonly ILogger Logger = Log.ForContext<EventStoreProcessManagerRepository<TProcess>>();

        public EventStoreProcessManagerRepository
        (
            EventStoreClient eventStoreClient,
            ICommandBus commandBus,
            Publisher publisher
        )
        {
            _eventStoreClient = eventStoreClient;
            _commandBus = commandBus;
            _publisher = publisher;
        }
        
        public async Task SaveAsync(TProcess processManager, CancellationToken cancellationToken)
        {
            if (processManager is null)
                throw new ArgumentNullException(nameof(processManager));

            string streamName = GetStreamName(processManager.Id);

            await _eventStoreClient.AppendToStreamWithRetryAsync
            (
                streamName,
                processManager.Version,
                processManager.GetAllMessages(),
                cancellationToken
            );

            try
            {
                foreach (ICommand command in processManager.GetOutgoingCommands())
                {
                    object commandResult = await _commandBus.SendAsync(command, cancellationToken);
                    
                    if (commandResult is IOneOf { Value: ApplicationError error })
                        throw new Exception(error.ToString());
                }
                
                // TODO - need to see if domain events can be created by process manager.
                // Publish domain events.
                // foreach (IDomainEvent domainEvent in processManager.GetOutgoingDomainEvents())
                //     await _publisher.Publish(@domainEvent, PublishStrategy.SyncStopOnException);
            }
            finally
            {
                processManager.ClearAllMessages();
            }
        }
        
        public async Task<TProcess> LoadAsync(Guid processManagerId, CancellationToken cancellationToken)
        {
            string streamName = GetStreamName(processManagerId);
            
            IList<IMessage> messages = await _eventStoreClient.ReadStreamForwardAsync<IMessage>
            (
                streamName,
                StreamPosition.Start,
                cancellationToken
            );

            TProcess processManager = new();
            processManager.Load(messages);

            return processManager;
        }

        private string GetStreamName(Guid processManagerId)
        {
            string processManagerName = typeof(TProcess).Name.Replace("ProcessManager", string.Empty);
            
            return $"{_eventStoreClient.ConnectionName}/process_manager/{processManagerName}/{processManagerId}".ToSnakeCase();
        }
    }
}