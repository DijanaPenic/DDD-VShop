using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Infrastructure;

public class SalesDispatcher : ISalesDispatcher
{
    public async Task<object> ExecuteCommandAsync<TCommand>
    (
        TCommand command,
        CancellationToken cancellationToken = default
    ) where TCommand : IBaseCommand
    {
        using IServiceScope scope = SalesCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }

    public Task PublishEventAsync<TEvent>
    (
        TEvent @event,
        CancellationToken cancellationToken = default
    ) where TEvent : IBaseEvent
    {
        using IServiceScope scope = SalesCompositionRoot.CreateScope();
        IEventDispatcher eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

        return eventDispatcher.PublishAsync(@event, cancellationToken);
    }
}