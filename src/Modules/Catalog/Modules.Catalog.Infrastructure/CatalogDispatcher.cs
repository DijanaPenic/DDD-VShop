using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Catalog.Infrastructure.Configuration;

namespace VShop.Modules.Catalog.Infrastructure;

public class CatalogDispatcher : ICatalogDispatcher
{
    public async Task<Result> SendAsync
    (
        ICommand command,
        CancellationToken cancellationToken = default
    )
    {
        using IServiceScope scope = CatalogCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }
    
    public async Task<Result<TResult>> SendAsync<TResult>
    (
        ICommand<TResult> command,
        CancellationToken cancellationToken = default
    )
    {
        using IServiceScope scope = CatalogCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task<Result<TResult>> QueryAsync<TResult>
    (
        IQuery<TResult> query,
        CancellationToken cancellationToken = default
    )
    {
        using IServiceScope scope = CatalogCompositionRoot.CreateScope();
        IQueryDispatcher queryDispatcher = scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();
        
        return await queryDispatcher.QueryAsync(query, cancellationToken);
    }
    
    public async Task PublishAsync
    (
        IBaseEvent @event,
        CancellationToken cancellationToken = default
    )
    {
        using IServiceScope scope = CatalogCompositionRoot.CreateScope();
        IEventDispatcher eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

        await eventDispatcher.PublishAsync(@event, cancellationToken);
    }
}