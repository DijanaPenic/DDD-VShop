using OneOf;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Commands;
using VShop.SharedKernel.Infrastructure.Messaging.Events.Publishing;
using VShop.SharedKernel.Infrastructure.Messaging.Commands.Publishing;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.EventSourcing.ProcessManagers;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventStore.Repositories
{
    public class EventStoreProcessManagerRepository<TProcess> : IProcessManagerRepository<TProcess>
        where TProcess : ProcessManager
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly ICommandBus _commandBus;
        private readonly Publisher _publisher;
        
        private static readonly ILogger Logger = Log.ForContext<EventStoreProcessManagerRepository<TProcess>>();

        public EventStoreProcessManagerRepository
        (
            IEventStoreConnection eventStoreConnection,
            ICommandBus commandBus,
            Publisher publisher
        )
        {
            _eventStoreConnection = eventStoreConnection;
            _commandBus = commandBus;
            _publisher = publisher;
        }
        
        public async Task SaveAsync(TProcess processManager)
        {
            if (processManager is null)
                throw new ArgumentNullException(nameof(processManager));

            string streamName = GetProcessManagerStreamName(processManager.Id);

            await _eventStoreConnection.AppendToStreamAsync
            (
                streamName,
                processManager.Version,
                processManager.GetAllMessages().ToArray()
            );

            try
            {
                foreach (ICommand command in processManager.GetOutgoingCommands())
                {
                    object commandResult = await _commandBus.SendAsync(command);
                    
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
        
        public async Task<bool> ExistsAsync(Guid processManagerId)
        {
            string streamName = GetProcessManagerStreamName(processManagerId);
            EventReadResult result = await _eventStoreConnection.ReadEventAsync(streamName, 1, false);
            
            return result.Status is not EventReadStatus.NoStream;
        }
        
        public async Task<TProcess> LoadAsync(Guid processManagerId)
        {
            string streamName = GetProcessManagerStreamName(processManagerId);
            List<IMessage> messages = await _eventStoreConnection.ReadStreamEventsForwardAsync<IMessage>(streamName);

            if (messages.Count is 0) return default;
                
            TProcess processManager = (TProcess)Activator.CreateInstance(typeof(TProcess), true);
            if (processManager is null)
                throw new Exception($"Couldn't resolve {nameof(TProcess)} instance.");

            processManager?.Load(messages);

            return processManager;
        }

        private string GetProcessManagerStreamName(Guid processManagerId)
        {
            string processManagerName = typeof(TProcess).Name.Replace("ProcessManager", string.Empty);
            
            return $"{_eventStoreConnection.ConnectionName}/process_manager/{processManagerName}/{processManagerId}".ToSnakeCase();
        }
    }
}