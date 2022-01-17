using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;

namespace VShop.SharedKernel.EventStoreDb.Subscriptions
{
    public interface ISubscriptionHandler
    {
        Task ProjectAsync
        (
            ResolvedEvent resolvedEvent,
            Func<SubscriptionDbContext, Task> checkpointUpdate,
            CancellationToken cancellationToken = default
        );
    }
}