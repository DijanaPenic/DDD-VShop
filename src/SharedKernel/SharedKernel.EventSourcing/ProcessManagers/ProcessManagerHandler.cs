using System;
using System.Threading.Tasks;
using Serilog;

using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.Infrastructure.Messaging.Events;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public abstract class ProcessManagerHandler<TProcess>
        where TProcess : ProcessManager
    {
        private readonly IProcessManagerRepository<TProcess> _processManagerRepository;
        
        private static readonly ILogger Logger = Log.ForContext<ProcessManagerHandler<TProcess>>();
        
        protected TProcess ProcessManager;

        protected ProcessManagerHandler(IProcessManagerRepository<TProcess> processManagerRepository)
            => _processManagerRepository = processManagerRepository;

        protected async Task TransitionAsync<TEvent>(Guid processId, Action transition)
            where TEvent : IEvent
        {
            Logger.Information
            (
                "{Process}: handling {Event} domain event",
                nameof(TProcess), nameof(TEvent)
            );
                
            ProcessManager = await _processManagerRepository.LoadAsync(processId);
            if (ProcessManager is null) throw new Exception("Process manager not found.");

            transition();
            
            await _processManagerRepository.SaveAsync(ProcessManager);
        }
    }
}