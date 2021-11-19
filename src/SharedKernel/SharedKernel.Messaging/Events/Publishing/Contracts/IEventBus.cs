using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VShop.SharedKernel.Messaging.Events.Publishing.Contracts
{
    public interface IEventBus
    {
        Task Publish<TNotification>(TNotification notification) 
            where TNotification : INotification;
        Task Publish<TNotification>(TNotification notification, EventPublishStrategy strategy)
            where TNotification : INotification;
        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken)
            where TNotification : INotification;
        Task Publish<TNotification>(TNotification notification, EventPublishStrategy strategy, CancellationToken cancellationToken)
            where TNotification : INotification;
    }
}