using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public abstract class ProcessManagerHandler<TProcess>
        where TProcess : ProcessManager
    {
        private readonly ILogger _logger;
        private readonly IClockService _clockService;
        private readonly IProcessManagerRepository<TProcess> _processManagerRepository;

        protected ProcessManagerHandler
        (
            ILogger logger,
            IClockService clockService,
            IProcessManagerRepository<TProcess> processManagerRepository
        )
        {
            _logger = logger;
            _clockService = clockService;
            _processManagerRepository = processManagerRepository;
        }
        
        protected async Task TransitionAsync(Guid processId, IBaseEvent @event, CancellationToken cancellationToken)
        {
            TProcess processManager = await _processManagerRepository.LoadAsync(processId, cancellationToken);
            
            _logger.Information
            (
                "{Process}: handling {Event} event",
                typeof(TProcess).Name, @event.GetType().Name
            );
            
            processManager.Transition(@event, _clockService);
            
            await _processManagerRepository.SaveAsync(processManager, cancellationToken);
        }
    }
}