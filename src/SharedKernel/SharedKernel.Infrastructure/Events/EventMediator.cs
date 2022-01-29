using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VShop.SharedKernel.Infrastructure.Events
{
    internal class EventMediator : Mediator
    {
        private readonly Func<IEnumerable<Func<INotification, CancellationToken, Task>>, INotification, CancellationToken, Task> _publish;

        public EventMediator
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