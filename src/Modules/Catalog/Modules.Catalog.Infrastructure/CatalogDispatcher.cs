using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Catalog.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;

namespace VShop.Modules.Catalog.Infrastructure;

public class CatalogDispatcher : ICatalogDispatcher
{
    public async Task<object> SendAsync<TCommand>
    (
        TCommand command,
        CancellationToken cancellationToken = default
    ) where TCommand : IBaseCommand
    {
        using IServiceScope scope = CatalogCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task<object> QueryAsync<TQuery>
    (
        TQuery query,
        CancellationToken cancellationToken = default
    ) where TQuery : IBaseQuery
    {
        using IServiceScope scope = CatalogCompositionRoot.CreateScope();
        IQueryDispatcher queryDispatcher = scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();
        
        return await queryDispatcher.QueryAsync(query, cancellationToken);
    }
    
    public async Task PublishAsync<TEvent>
    (
        TEvent @event,
        CancellationToken cancellationToken = default
    ) where TEvent : IBaseEvent
    {
        using IServiceScope scope = CatalogCompositionRoot.CreateScope();
        IEventDispatcher eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

        await eventDispatcher.PublishAsync(@event, cancellationToken);
    }
}