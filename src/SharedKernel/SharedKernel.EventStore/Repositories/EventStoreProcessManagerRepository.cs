using OneOf;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventStore.Helpers;
using VShop.SharedKernel.EventStore.Extensions;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Messaging;
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
        
        private static readonly ILogger Logger = Log.ForContext<EventStoreProcessManagerRepository<TProcess>>();

        public EventStoreProcessManagerRepository
        (
            IEventStoreConnection eventStoreConnection,
            ICommandBus commandBus
        )
        {
            _eventStoreConnection = eventStoreConnection;
            _commandBus = commandBus;
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
                EventStoreHelper.PrepareMessageData(processManager.GetAllMessages().ToArray())
            );

            try
            {
                foreach (IMessage command in processManager.GetCommands())
                {
                    object commandResult = await _commandBus.SendAsync(command);
                    if (commandResult is IOneOf { Value: ApplicationError error }) 
                        throw new Exception(error.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unhandled error has occurred");
                
                throw;
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
            
            return result.Status != EventReadStatus.NoStream;
        }
        
        public async Task<TProcess> LoadAsync(Guid processManagerId)
        {
            string streamName = GetProcessManagerStreamName(processManagerId);
            List<IMessage> events = await _eventStoreConnection.ReadStreamEventsForwardAsync<IMessage>(streamName);

            if (events.Count is 0) return default;
                
            TProcess processManager = (TProcess)Activator.CreateInstance(typeof(TProcess), true);
            if (processManager is null)
                throw new Exception($"Couldn't resolve {nameof(TProcess)} instance.");

            processManager?.Load(events);

            return processManager;
        }

        private string GetProcessManagerStreamName(Guid processManagerId)
        {
            string processManagerName = typeof(TProcess).Name.Replace("ProcessManager", string.Empty);
            
            return $"{_eventStoreConnection.ConnectionName}/process_manager/{processManagerName}/{processManagerId}".ToSnakeCase();
        }
    }
}