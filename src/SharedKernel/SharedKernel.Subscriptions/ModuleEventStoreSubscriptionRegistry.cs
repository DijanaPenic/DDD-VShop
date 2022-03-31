using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Subscriptions.Services.Contracts;

namespace VShop.SharedKernel.Subscriptions;

public static class ModuleEventStoreSubscriptionRegistry
{
    private static readonly List<IEventStoreBackgroundService> Registrations = new();
    
    public static IEnumerable<IEventStoreBackgroundService> Services => Registrations;
    
    public static void Add(IServiceProvider serviceProvider) 
        => Registrations.AddRange(serviceProvider.GetServices<IEventStoreBackgroundService>());
}