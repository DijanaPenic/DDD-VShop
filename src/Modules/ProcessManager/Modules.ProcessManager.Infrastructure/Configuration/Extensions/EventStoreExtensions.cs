using Serilog;
using EventStore.Client;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventSourcing.Stores;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Integration.Projections;
using VShop.SharedKernel.Integration.Stores;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Services;
using VShop.SharedKernel.Subscriptions.Services.Contracts;

[assembly: InternalsVisibleTo("VShop.Modules.ProcessManager.API")]
namespace VShop.Modules.ProcessManager.Infrastructure.Configuration.Extensions;

internal static class EventStoreExtensions
{
    public static void AddEventStore(this IServiceCollection services, string connectionString)
    {
        EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(connectionString);
        eventStoreSettings.ConnectionName = "ProcessManager";

        EventStoreClient eventStoreClient = new(eventStoreSettings);
        services.AddSingleton(eventStoreClient);
        
        services.AddSingleton<CustomEventStoreClient>();
        
        services.AddTransient
        (
            typeof(IIntegrationEventStore),
            typeof(IntegrationEventStore)
        );
        services.AddTransient
        (
            typeof(IProcessManagerStore<>),
            typeof(ProcessManagerStore<>)
        );

        // Stream names
        string processManagerStreamPrefix = $"{eventStoreClient.ConnectionName}".ToSnakeCase();
        
        // Publish integration events from the current bounded context
        services.AddSingleton<IntegrationEventProjectionToEventStore>();
        services.AddSingleton<IEventStoreBackgroundService, EventStoreBackgroundService>(provider =>
        {
            ILogger logger = provider.GetService<ILogger>();
            IMessageRegistry messageRegistry = provider.GetService<IMessageRegistry>();
            ISubscriptionHandler subscriptionHandler = provider.GetService<IntegrationEventProjectionToEventStore>();
            IMessageContextRegistry messageContextRegistry = provider.GetService<IMessageContextRegistry>();

            return new EventStoreBackgroundService
            (
                logger, eventStoreClient, provider, messageRegistry, messageContextRegistry,
                new SubscriptionConfig
                (
                    "IntegrationEventsPub", 
                    subscriptionHandler,
                    new SubscriptionFilterOptions(StreamFilter.RegularExpression
                        (new Regex($"^{processManagerStreamPrefix}.*outbox")))
                )
            );
        });

        // Subscribe to integration streams
        services.AddSingleton<IntegrationEventPublisher>();
        services.AddSingleton<IEventStoreBackgroundService, EventStoreBackgroundService>(provider =>
        {
            ILogger logger = provider.GetService<ILogger>();
            IMessageRegistry messageRegistry = provider.GetService<IMessageRegistry>();
            ISubscriptionHandler subscriptionHandler = provider.GetService<IntegrationEventPublisher>();
            IMessageContextRegistry messageContextRegistry = provider.GetService<IMessageContextRegistry>();

            return new EventStoreBackgroundService
            (
                logger, eventStoreClient, provider, messageRegistry, messageContextRegistry,
                new SubscriptionConfig
                (
                    "IntegrationEventsSub", 
                    subscriptionHandler,
                    new SubscriptionFilterOptions(StreamFilter.RegularExpression(new Regex(@".*\/integration$")))
                )
            );
        });
    }
}