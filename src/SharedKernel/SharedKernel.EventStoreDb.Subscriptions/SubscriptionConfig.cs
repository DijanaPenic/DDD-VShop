using EventStore.Client;

namespace VShop.SharedKernel.EventStoreDb.Subscriptions
{
    public class SubscriptionConfig
    {
        public string SubscriptionName { get; }
        public ISubscriptionHandler SubscriptionHandler { get; }
        public SubscriptionFilterOptions SubscriptionFilterOptions { get; }

        public SubscriptionConfig
        (   
            string connectionName,
            string subscriptionId,
            ISubscriptionHandler subscriptionHandler,
            SubscriptionFilterOptions filterOptions = default
        )
        {
            SubscriptionHandler = subscriptionHandler;
            SubscriptionName = $"{connectionName}-{subscriptionId}";
            SubscriptionFilterOptions = filterOptions ?? new SubscriptionFilterOptions(EventTypeFilter.ExcludeSystemEvents());
        }
    }
}