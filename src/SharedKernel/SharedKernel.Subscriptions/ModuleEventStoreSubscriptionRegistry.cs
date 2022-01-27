using VShop.SharedKernel.Subscriptions.Services.Contracts;

namespace VShop.SharedKernel.Subscriptions;

public static class ModuleEventStoreSubscriptionRegistry
{
    private static readonly List<IEventStoreBackgroundService> Registry = new();
    public static IList<IEventStoreBackgroundService> Services => Registry;

    public static void Add(IEnumerable<IEventStoreBackgroundService> services) => Registry.AddRange(services);
}