using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Catalog.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Catalog.Infrastructure;

public class CatalogDispatcher : ICatalogDispatcher
{
    public async Task<object> ExecuteCommandAsync<TCommand>
    (
        TCommand command,
        CancellationToken cancellationToken = default
    ) where TCommand : IBaseCommand
    {
        using IServiceScope scope = CatalogCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }

    public Task PublishEventAsync<TEvent>
    (
        TEvent @event,
        CancellationToken cancellationToken = default
    ) where TEvent : IBaseEvent
    {
        using IServiceScope scope = CatalogCompositionRoot.CreateScope();
        IEventDispatcher eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

        return eventDispatcher.PublishAsync(@event, cancellationToken);
    }
}