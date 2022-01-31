using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Billing.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.Modules.Billing.Infrastructure;

public class BillingDispatcher : IBillingDispatcher
{
    public async Task<object> ExecuteCommandAsync<TCommand>
    (
        TCommand command,
        IMessageContext messageContext,
        CancellationToken cancellationToken = default
    ) where TCommand : IBaseCommand
    {
        using IServiceScope scope = BillingCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();
        IMessageContextRegistry messageContextRegistry = scope.ServiceProvider.GetRequiredService<IMessageContextRegistry>();

        messageContextRegistry.Set((IMessage)command, messageContext);
        return await commandDispatcher.SendAsync(command, cancellationToken);
    }

    public Task PublishEventAsync<TEvent>
    (
        TEvent @event,
        IMessageContext messageContext,
        CancellationToken cancellationToken = default
    ) where TEvent : IBaseEvent
    {
        using IServiceScope scope = BillingCompositionRoot.CreateScope();
        IEventDispatcher eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();
        IMessageContextRegistry messageContextRegistry = scope.ServiceProvider.GetRequiredService<IMessageContextRegistry>();

        messageContextRegistry.Set(@event, messageContext);
        return eventDispatcher.PublishAsync(@event, cancellationToken);
    }
}