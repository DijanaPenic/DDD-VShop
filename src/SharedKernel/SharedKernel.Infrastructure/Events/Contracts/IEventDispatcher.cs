using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Events.Contracts;

public interface IEventDispatcher
{
    Task PublishAsync<TEvent>
    (
        TEvent notification,
        EventDispatchStrategy strategy,
        CancellationToken cancellationToken = default
    )
        where TEvent : IBaseEvent;
}