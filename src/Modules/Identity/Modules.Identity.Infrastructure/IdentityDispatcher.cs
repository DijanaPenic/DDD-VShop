using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure;

namespace VShop.Modules.Identity.Infrastructure;

public class IdentityDispatcher : IIdentityDispatcher
{
    public async Task<Result> SendAsync
    (
        ICommand command,
        CancellationToken cancellationToken = default
    )
    {
        using IServiceScope scope = IdentityCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task<Result<TResult>> QueryAsync<TResult>
    (
        IQuery<TResult> query,
        CancellationToken cancellationToken = default
    )
    {
        using IServiceScope scope = IdentityCompositionRoot.CreateScope();
        IQueryDispatcher queryDispatcher = scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();
        
        return await queryDispatcher.QueryAsync(query, cancellationToken);
    }

    public async Task PublishAsync<TEvent>
    (
        TEvent @event,
        CancellationToken cancellationToken = default
    ) where TEvent : IBaseEvent
    {
        using IServiceScope scope = IdentityCompositionRoot.CreateScope();
        IEventDispatcher eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

        await eventDispatcher.PublishAsync(@event, cancellationToken);
    }
}