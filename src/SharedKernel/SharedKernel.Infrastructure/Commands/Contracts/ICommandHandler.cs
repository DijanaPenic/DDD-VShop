using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Commands.Contracts
{
    public interface ICommandHandler<in TCommand, TResponse> where TCommand : class, ICommand<TResponse>
    {
        Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken);
    }

    public interface ICommandHandler<in TCommand> where TCommand : class, ICommand
    {
        Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}