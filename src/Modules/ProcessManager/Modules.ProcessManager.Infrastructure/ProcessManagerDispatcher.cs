using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.ProcessManager.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure;

public class ProcessManagerDispatcher : IProcessManagerDispatcher
{
    public async Task<object> ExecuteCommandAsync<TCommand>
    (
        TCommand command,
        CancellationToken cancellationToken = default
    ) where TCommand : IBaseCommand
    {
        using IServiceScope scope = ProcessManagerCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }

    public Task PublishEventAsync<TEvent>
    (
        TEvent @event,
        CancellationToken cancellationToken = default
    ) where TEvent : IBaseEvent
    {
        using IServiceScope scope = ProcessManagerCompositionRoot.CreateScope();
        IEventDispatcher eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

        return eventDispatcher.PublishAsync(@event, cancellationToken);
    }
}