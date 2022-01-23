using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;

namespace VShop.SharedKernel.Infrastructure.Dispatchers
{
    public class NotificationMediator : Mediator
    {
        private readonly Func<IEnumerable<Func<INotification, CancellationToken, Task>>, INotification, CancellationToken, Task> _publish;

        public NotificationMediator
        (
            ServiceFactory serviceFactory,
            Func<IEnumerable<Func<INotification, CancellationToken, Task>>, INotification, CancellationToken, Task>
                publish
        ) : base(serviceFactory) => _publish = publish;

        protected override Task PublishCore
        (
            IEnumerable<Func<INotification, CancellationToken, Task>> allHandlers,
            INotification notification,
            CancellationToken cancellationToken
        ) => _publish(allHandlers, notification, cancellationToken);
    }
}