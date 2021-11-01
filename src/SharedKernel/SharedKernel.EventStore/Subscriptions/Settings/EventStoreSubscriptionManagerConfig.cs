using VShop.SharedKernel.EventSourcing;

namespace VShop.SharedKernel.EventStore.Subscriptions.Settings
{
    public record EventStoreSubscriptionManagerConfig
    {
        public ISubscription[] Handlers { get; }
        public string Name { get; }

        public EventStoreSubscriptionManagerConfig(string name, params ISubscription[] handlers)
        {
            Name = name;
            Handlers = handlers;
        }
    }
}