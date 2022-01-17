using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Messaging.Commands.Publishing.Contracts
{
    public interface ICommandBus
    {
        Task<object> SendAsync(object command, CancellationToken cancellationToken = default);
        Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default);
        Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    }
}