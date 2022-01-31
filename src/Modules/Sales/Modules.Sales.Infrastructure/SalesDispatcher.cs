using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Dispatchers;

namespace VShop.Modules.Sales.Infrastructure;

public class SalesDispatcher : IDispatcher
{
    public async Task<object> ExecuteCommandAsync(object command, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = SalesCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }
}