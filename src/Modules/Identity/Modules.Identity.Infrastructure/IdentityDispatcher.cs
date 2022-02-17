using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Configuration;

namespace VShop.Modules.Identity.Infrastructure;

public class IdentityDispatcher : IIdentityDispatcher
{
    public Task<object> SendAsync<TCommand>
    (
        TCommand command,
        CancellationToken cancellationToken = default
    ) where TCommand : IBaseCommand
    {
        using IServiceScope scope = IdentityCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return commandDispatcher.SendAsync(command, cancellationToken);
    }

    public Task<object> QueryAsync<TQuery>
    (
        TQuery query,
        CancellationToken cancellationToken = default
    ) where TQuery : IBaseQuery
    {
        using IServiceScope scope = IdentityCompositionRoot.CreateScope();
        IQueryDispatcher queryDispatcher = scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();
        
        return queryDispatcher.QueryAsync(query, cancellationToken);
    }

    public Task PublishAsync<TEvent>
    (
        TEvent @event,
        CancellationToken cancellationToken = default
    ) where TEvent : IBaseEvent
    {
        using IServiceScope scope = IdentityCompositionRoot.CreateScope();
        IEventDispatcher eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

        return eventDispatcher.PublishAsync(@event, cancellationToken);
    }
}