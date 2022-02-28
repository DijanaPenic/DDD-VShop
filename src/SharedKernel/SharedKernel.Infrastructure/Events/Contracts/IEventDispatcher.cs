using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Events.Contracts;

public interface IEventDispatcher
{
    Task PublishAsync
    (
        IBaseEvent @event,
        EventDispatchStrategy strategy,
        CancellationToken cancellationToken
    );
    
    Task PublishAsync
    (
        IBaseEvent @event,
        CancellationToken cancellationToken
    );
}