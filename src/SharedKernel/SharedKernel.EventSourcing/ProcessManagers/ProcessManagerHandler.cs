using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public abstract class ProcessManagerHandler<TProcess>
        where TProcess : ProcessManager
    {
        private readonly ILogger _logger;
        private readonly IProcessManagerRepository<TProcess> _processManagerRepository;

        protected ProcessManagerHandler(ILogger logger, IProcessManagerRepository<TProcess> processManagerRepository)
        {
            _logger = logger;
            _processManagerRepository = processManagerRepository;
        }

        protected async Task<Result> ExecuteAsync(Guid processId, IBaseCommand command, CancellationToken cancellationToken)
        {
            TProcess processManager = await _processManagerRepository.LoadAsync(processId, cancellationToken);
            
            _logger.Information
            (
                "{Process}: handling {Command} command",
                typeof(TProcess).Name, command.GetType().Name
            );
            
            processManager.Execute(command);
            
            await _processManagerRepository.SaveAsync(processManager, cancellationToken);

            return Result.Success;
        }
        
        protected async Task TransitionAsync(Guid processId, IBaseEvent @event, CancellationToken cancellationToken)
        {
            TProcess processManager = await _processManagerRepository.LoadAsync(processId, cancellationToken);
            
            _logger.Information
            (
                "{Process}: handling {Event} event",
                typeof(TProcess).Name, @event.GetType().Name
            );
            
            processManager.Transition(@event);
            
            await _processManagerRepository.SaveAsync(processManager, cancellationToken);
        }
    }
}