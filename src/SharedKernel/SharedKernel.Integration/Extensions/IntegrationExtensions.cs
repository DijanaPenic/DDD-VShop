using EventStore.Client;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Integration.Projections;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Extensions;

namespace VShop.SharedKernel.Integration.Extensions;

public static class IntegrationExtensions
{
    public static IServiceCollection AddIntegrationEventPubBackgroundService
    (
        this IServiceCollection services,
        string filterPattern
    )
    {
        services.AddSingleton<IntegrationEventProjectionToEventStore>();
        services.AddEventStoreBackgroundService(provider => new SubscriptionConfig
        (
            "IntegrationEventPub",
            provider.GetService<IntegrationEventProjectionToEventStore>(),
            new SubscriptionFilterOptions(StreamFilter.RegularExpression(new Regex(filterPattern)))
        ));
        
        return services;
    }
    
    public static IServiceCollection AddIntegrationEventSubBackgroundService(this IServiceCollection services)
    {
        services.AddSingleton<IntegrationEventPublisher>();
        services.AddEventStoreBackgroundService(provider => new SubscriptionConfig
        (
            "IntegrationEventSub",
            provider.GetService<IntegrationEventPublisher>(),
            new SubscriptionFilterOptions(StreamFilter.RegularExpression(new Regex(@".*\/integration$")))
        ));
        
        return services;
    }
}