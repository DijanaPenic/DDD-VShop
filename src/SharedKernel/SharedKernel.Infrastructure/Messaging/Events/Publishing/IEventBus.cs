using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Messaging.Events.Publishing
{
    public interface IEventBus
    {
        Task Publish<TNotification>(TNotification notification);
        Task Publish<TNotification>(TNotification notification, EventPublishStrategy strategy);
        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken);
        Task Publish<TNotification>(TNotification notification, EventPublishStrategy strategy, CancellationToken cancellationToken);
    }
}