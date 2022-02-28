using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Commands.Contracts
{
    public interface ICommandDispatcher
    {
        Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default);
        Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    }
}