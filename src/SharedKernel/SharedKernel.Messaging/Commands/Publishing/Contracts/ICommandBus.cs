using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Messaging.Commands.Publishing.Contracts
{
    public interface ICommandBus
    {
        Task<object> SendAsync(object command, CancellationToken cancellationToken = default);

        Task<Result> SendAsync<TCommand>
        (
            IdentifiedCommand<TCommand> command,
            CancellationToken cancellationToken = default
        ) where TCommand : IBaseCommand;
        
        Task<Result<TResponse>> SendAsync<TCommand, TResponse>
        (
            IdentifiedCommand<TCommand, TResponse> command,
            CancellationToken cancellationToken = default
        ) where TCommand : IBaseCommand;
    }
}