using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Commands.Publishing.Contracts
{
    public interface ICommandBus
    {
        Task<object> SendAsync(object command, CancellationToken cancellationToken = default);
        Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default);
        Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    }
}