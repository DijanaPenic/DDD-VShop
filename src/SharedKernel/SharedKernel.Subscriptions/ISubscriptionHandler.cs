using EventStore.Client;

using VShop.SharedKernel.Subscriptions.DAL;

namespace VShop.SharedKernel.Subscriptions
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