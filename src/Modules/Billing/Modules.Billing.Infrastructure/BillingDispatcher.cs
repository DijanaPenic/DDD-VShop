using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Billing.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Billing.Infrastructure;

public class BillingDispatcher : IBillingDispatcher
{
    public async Task<object> ExecuteCommandAsync(object command, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = BillingCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }
}