using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.SharedKernel.EventSourcing.ProcessManagers
{
    public abstract class ProcessManagerHandler<TProcess> where TProcess : ProcessManager, new()
    {
        private readonly IClockService _clockService;
        private readonly ILogger _logger;
        private readonly IProcessManagerStore<TProcess> _processManagerStore;

        protected ProcessManagerHandler
        (
            IClockService clockService,
            ILogger logger,
            IProcessManagerStore<TProcess> processManagerStore
        )
        {
            _clockService = clockService;
            _logger = logger;
            _processManagerStore = processManagerStore;
        }

        protected async Task TransitionAsync
        (
            Guid processId,
            IBaseEvent @event,
            CancellationToken cancellationToken
        )
        {
            // TODO - missing validation for invalid processId.
            
            _logger.Information
            (
                "{Process}: handling {Event} event",
                typeof(TProcess).Name, @event.GetType().Name
            );

            TProcess processManager = await _processManagerStore.LoadAsync
            (
                processId,
                @event.Metadata.MessageId,
                cancellationToken
            );

            if (processManager.IsRestored) return;
            
            processManager.Transition(@event, _clockService.Now);

            await _processManagerStore.SaveAndPublishAsync
            (
                processManager,
                @event.Metadata.MessageId,
                @event.Metadata.CorrelationId,
                cancellationToken
            );
        }
    }
}