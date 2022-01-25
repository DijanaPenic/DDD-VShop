using EventStore.Client;

namespace VShop.SharedKernel.Subscriptions
{
    public class SubscriptionConfig
    {
        public string SubscriptionId { get; }
        public ISubscriptionHandler SubscriptionHandler { get; }
        public SubscriptionFilterOptions SubscriptionFilterOptions { get; }

        public SubscriptionConfig
        (
            string subscriptionId,
            ISubscriptionHandler subscriptionHandler,
            SubscriptionFilterOptions filterOptions = default
        )
        {
            SubscriptionHandler = subscriptionHandler;
            SubscriptionId = subscriptionId;
            SubscriptionFilterOptions = filterOptions ?? new SubscriptionFilterOptions(EventTypeFilter.ExcludeSystemEvents());
        }
    }
}