using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing;

namespace VShop.SharedKernel.EventStore.Subscriptions.Settings
{
    public record EventStoreSubscriptionManagerFilteredConfig : EventStoreSubscriptionManagerConfig
    {
        public Filter Filter { get; }

        public EventStoreSubscriptionManagerFilteredConfig
        (
            string name,
            Filter filter,
            params ISubscription[] handlers
        )
            : base(name, handlers)
        {
            Filter = filter;
        }
    }
}