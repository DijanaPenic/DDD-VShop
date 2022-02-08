using Serilog;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Subscriptions.Services;
using VShop.SharedKernel.Subscriptions.Services.Contracts;
using VShop.SharedKernel.EventStoreDb.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Subscriptions.Extensions;

public static class SubscriptionExtensions
{
    // TODO - refactor.
    public static IServiceCollection AddEventStoreBackgroundService
    (
        this IServiceCollection services,
        Func<IServiceProvider, SubscriptionConfig> configFactory
    )
    {
        services.AddSingleton<IEventStoreBackgroundService, EventStoreBackgroundService>(provider =>
        {
            ILogger logger = provider.GetService<ILogger>();
            EventStoreClient eventStoreClient = provider.GetService<EventStoreClient>();
            IMessageRegistry messageRegistry = provider.GetService<IMessageRegistry>();
            IEventStoreMessageConverter eventStoreMessageConverter = provider.GetService<IEventStoreMessageConverter>();
            IMessageContextRegistry messageContextRegistry = provider.GetService<IMessageContextRegistry>();

            return new EventStoreBackgroundService
            (
                logger, eventStoreClient, provider, messageRegistry, 
                eventStoreMessageConverter, messageContextRegistry,
                configFactory(provider)
            );
        });
        
        return services;
    }

}