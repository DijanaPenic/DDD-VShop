using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.SharedKernel.Infrastructure.Dispatchers;

public interface IDispatcher
{
    Task<object> SendAsync(object command, CancellationToken cancellationToken = default);
    Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default);
    Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
}