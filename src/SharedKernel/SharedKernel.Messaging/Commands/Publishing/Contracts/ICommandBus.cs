using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.SharedKernel.Messaging.Commands.Publishing.Contracts
{
    public interface ICommandBus
    {
        Task<object> SendAsync(object command, CancellationToken cancellationToken = default);
        //Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default);
        Task<Result<TResponse>> SendAsync<TCommand, TResponse>
        (
            IIdentifiedCommand<TCommand, TResponse> command,
            CancellationToken cancellationToken = default
        ) where TCommand : IBaseCommand;
    }
}