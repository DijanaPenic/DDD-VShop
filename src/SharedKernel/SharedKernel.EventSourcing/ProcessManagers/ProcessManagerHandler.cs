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
        private TProcess _processManager;
        private readonly IProcessManagerRepository<TProcess> _processManagerRepository;

        private static readonly ILogger Logger = Log.ForContext<ProcessManagerHandler<TProcess>>();

        protected ProcessManagerHandler(IProcessManagerRepository<TProcess> processManagerRepository)
            => _processManagerRepository = processManagerRepository;
        
        protected async Task<Result> ExecuteAsync(Guid processId, IBaseCommand command, CancellationToken cancellationToken)
        {
            _processManager = await _processManagerRepository.LoadAsync(processId, cancellationToken);
            
            Logger.Information
            (
                "{Process}: handling {Command} command",
                typeof(TProcess).Name, command.GetType().Name
            );
            
            _processManager.Execute(command);
            
            await _processManagerRepository.SaveAsync(_processManager, cancellationToken);

            return Result.Success;
        }
        
        protected async Task TransitionAsync(Guid processId, IBaseEvent @event, CancellationToken cancellationToken)
        {
            _processManager = await _processManagerRepository.LoadAsync(processId, cancellationToken);
            
            Logger.Information
            (
                "{Process}: handling {Event} event",
                typeof(TProcess).Name, @event.GetType().Name
            );
            
            _processManager.Transition(@event);
            
            await _processManagerRepository.SaveAsync(_processManager, cancellationToken);
        }
    }
}