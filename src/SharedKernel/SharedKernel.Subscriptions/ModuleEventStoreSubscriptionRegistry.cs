using VShop.SharedKernel.Subscriptions.Services.Contracts;

namespace VShop.SharedKernel.Subscriptions;

public static class ModuleEventStoreSubscriptionRegistry
{
    private static readonly List<IEventStoreBackgroundService> _registry = new();
    public static IList<IEventStoreBackgroundService> Services => _registry;

    public static void Add(IEnumerable<IEventStoreBackgroundService> services) => _registry.AddRange(services);
}