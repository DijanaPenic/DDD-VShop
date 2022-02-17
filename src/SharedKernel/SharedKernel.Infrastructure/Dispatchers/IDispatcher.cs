using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.SharedKernel.Infrastructure.Dispatchers;

public interface IDispatcher
{
    Task<object> SendAsync<TCommand>
    (
        TCommand command,
        CancellationToken cancellationToken = default
    ) where TCommand : IBaseCommand;

    Task<object> QueryAsync<TQuery>
    (
        TQuery query,
        CancellationToken cancellationToken = default
    ) where TQuery : IBaseQuery;
    
    Task PublishAsync<TEvent>
    (
        TEvent @event,
        CancellationToken cancellationToken = default
    ) where TEvent : IBaseEvent;
}