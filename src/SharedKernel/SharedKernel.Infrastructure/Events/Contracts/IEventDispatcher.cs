using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Dispatchers;

namespace VShop.SharedKernel.Infrastructure.Events.Contracts;

public interface IEventDispatcher
{
    Task PublishAsync<TNotification>(TNotification notification) 
        where TNotification : INotification;
    Task PublishAsync<TNotification>(TNotification notification, NotificationDispatchStrategy strategy)
        where TNotification : INotification;
    Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken)
        where TNotification : INotification;
    Task PublishAsync<TNotification>(TNotification notification, NotificationDispatchStrategy strategy, CancellationToken cancellationToken)
        where TNotification : INotification;
}