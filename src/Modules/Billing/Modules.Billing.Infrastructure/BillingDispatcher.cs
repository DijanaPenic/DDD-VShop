using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Billing.Infrastructure.Configuration;

namespace VShop.Modules.Billing.Infrastructure;

internal class BillingDispatcher : IBillingDispatcher
{
    public async Task<object> SendAsync(object command, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = BillingCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = BillingCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task<Result<TResponse>> SendAsync<TResponse>
    (
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default
    )
    {
        using IServiceScope scope = BillingCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }
}