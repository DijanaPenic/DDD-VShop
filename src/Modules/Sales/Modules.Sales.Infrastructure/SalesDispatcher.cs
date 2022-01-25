using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Sales.Infrastructure.Configuration;

namespace VShop.Modules.Sales.Infrastructure;

public class SalesDispatcher : ISalesDispatcher
{
    public async Task<object> SendAsync(object command, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = SalesCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = SalesCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }

    public async Task<Result<TResponse>> SendAsync<TResponse>
    (
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default
    )
    {
        using IServiceScope scope = SalesCompositionRoot.CreateScope();
        ICommandDispatcher commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        return await commandDispatcher.SendAsync(command, cancellationToken);
    }
}