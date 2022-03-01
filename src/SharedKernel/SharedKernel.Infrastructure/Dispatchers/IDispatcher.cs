using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.SharedKernel.Infrastructure.Dispatchers;

public interface IDispatcher
{
    Task<Result> SendAsync
    (
        ICommand command,
        CancellationToken cancellationToken = default
    );
    
    Task<Result<TResult>> SendAsync<TResult>
    (
        ICommand<TResult> command,
        CancellationToken cancellationToken = default
    );

    Task<Result<TResult>> QueryAsync<TResult>
    (
        IQuery<TResult> query,
        CancellationToken cancellationToken = default
    );
    
    Task PublishAsync
    (
        IBaseEvent @event,
        CancellationToken cancellationToken = default
    );
}