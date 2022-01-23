using EventStore.Client;

using VShop.SharedKernel.EventStoreDb.Subscriptions.DAL;

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