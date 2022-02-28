using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Events.Contracts
{
    public interface IEventHandler<in TEvent> where TEvent : class, IBaseEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}