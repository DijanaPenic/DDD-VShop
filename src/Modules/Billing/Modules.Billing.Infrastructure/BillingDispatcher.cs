using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Billing.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;

namespace VShop.Modules.Billing.Infrastructure;

public class BillingDispatcher : IBillingDispatcher
{
    public async Task<object> SendAsync<TCommand>
    (
        TCommand command,
        CancellationToken cancellationToken = default
    ) where TCommand : IBaseCommand
    {
        using IServiceScope scope = BillingCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }

    public Task<object> QueryAsync<TQuery>
    (
        TQuery query,
        CancellationToken cancellationToken = default
    ) where TQuery : IBaseQuery
    {
        using IServiceScope scope = BillingCompositionRoot.CreateScope();
        IQueryDispatcher queryDispatcher = scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();
        
        return queryDispatcher.QueryAsync(query, cancellationToken);
    }
    
    public Task PublishAsync<TEvent>
    (
        TEvent @event,
        CancellationToken cancellationToken = default
    ) where TEvent : IBaseEvent
    {
        using IServiceScope scope = BillingCompositionRoot.CreateScope();
        IEventDispatcher eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();
     
        return eventDispatcher.PublishAsync(@event, cancellationToken);
    }
}