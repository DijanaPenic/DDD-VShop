using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Infrastructure.Dispatchers;

public interface IDispatcher
{
    Task<object> ExecuteCommandAsync<TCommand>
    (
        TCommand command,
        CancellationToken cancellationToken = default
    ) where TCommand : IBaseCommand;

    Task PublishEventAsync<TEvent>
    (
        TEvent @event,
        CancellationToken cancellationToken = default
    ) where TEvent : IBaseEvent;
}