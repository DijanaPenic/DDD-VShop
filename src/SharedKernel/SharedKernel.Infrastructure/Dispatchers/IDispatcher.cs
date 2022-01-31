using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Dispatchers;

public interface IDispatcher
{
    Task<object> ExecuteCommandAsync<TCommand>
    (
        TCommand command,
        IMessageContext messageContext,
        CancellationToken cancellationToken = default
    ) where TCommand : IBaseCommand;

    Task PublishEventAsync<TEvent>
    (
        TEvent @event,
        IMessageContext messageContext,
        CancellationToken cancellationToken = default
    ) where TEvent : IBaseEvent;
}