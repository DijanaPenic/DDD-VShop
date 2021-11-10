using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.Infrastructure.Messaging.Events;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public abstract class ProcessManagerHandler<TProcess>
        where TProcess : ProcessManager
    {
        private TProcess _processManager;
        private readonly IProcessManagerRepository<TProcess> _processManagerRepository;

        private static readonly ILogger Logger = Log.ForContext<ProcessManagerHandler<TProcess>>();

        protected ProcessManagerHandler(IProcessManagerRepository<TProcess> processManagerRepository)
            => _processManagerRepository = processManagerRepository;

        // TODO - handle cancellation token
        protected async Task TransitionAsync(Guid processId, IEvent @event, CancellationToken _)
        {
            _processManager = await _processManagerRepository.LoadAsync(processId);
            
            Logger.Information
            (
                "{Process}: handling {Event} domain event",
                nameof(_processManager), nameof(@event)
            );
            
            _processManager.Transition(@event);
            
            await _processManagerRepository.SaveAsync(_processManager);
        }
    }
}