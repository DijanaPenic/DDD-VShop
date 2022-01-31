using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Catalog.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Catalog.Infrastructure;

public class CatalogDispatcher : ICatalogDispatcher
{
    public async Task<object> ExecuteCommandAsync(object command, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = CatalogCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }
}