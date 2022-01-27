using Serilog;
using EventStore.Client;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Services;
using VShop.SharedKernel.Subscriptions.Services.Contracts;
using VShop.SharedKernel.Integration.Stores;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Integration.Projections;
using VShop.SharedKernel.EventSourcing.Stores;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Application.Projections;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.Modules.Sales.Infrastructure.DAL;
using VShop.Modules.Sales.Infrastructure.Projections;

[assembly: InternalsVisibleTo("VShop.Modules.Sales.API")]
namespace VShop.Modules.Sales.Infrastructure.Configuration.Extensions;

internal static class EventStoreExtensions
{
    public static void AddEventStore(this IServiceCollection services, string connectionString)
    {
        EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(connectionString);
        eventStoreSettings.ConnectionName = "Sales";

        EventStoreClient eventStoreClient = new(eventStoreSettings);
        services.AddSingleton(eventStoreClient);
        
        services.AddSingleton<CustomEventStoreClient>();

        services.AddSingleton
        (
            typeof(IAggregateStore<>),
            typeof(AggregateStore<>)
        );
        services.AddSingleton
        (typeof(IIntegrationEventStore),
            typeof(IntegrationEventStore)
        );
        services.AddScoped
        (typeof(IProcessManagerStore<>),
            typeof(ProcessManagerStore<>)
        );

        // Stream names
        string aggregateStreamPrefix = $"{eventStoreClient.ConnectionName}/aggregate".ToSnakeCase();
        string processManagerStreamPrefix = $"{eventStoreClient.ConnectionName}/process_manager".ToSnakeCase();

        // Read model projections
        services.AddSingleton<IEventStoreBackgroundService, EventStoreBackgroundService>(provider =>
        {
            ILogger logger = provider.GetService<ILogger>();
            IMessageRegistry messageRegistry = provider.GetService<IMessageRegistry>();
            
            return new EventStoreBackgroundService
            (
                logger, eventStoreClient, provider, messageRegistry,
                new SubscriptionConfig
                (
                    "ReadModels",
                    new DomainEventProjectionToPostgres<SalesDbContext>
                    (
                        logger, provider, messageRegistry,
                        ShoppingCartInfoProjection.ProjectAsync
                    ),
                    new SubscriptionFilterOptions(StreamFilter.Prefix(aggregateStreamPrefix))
                )
            );
        });

        // Publish integration events from the current bounded context
        services.AddSingleton<IntegrationEventProjectionToEventStore>();
        services.AddSingleton<IEventStoreBackgroundService, EventStoreBackgroundService>(provider =>
        {
            ILogger logger = provider.GetService<ILogger>();
            IMessageRegistry messageRegistry = provider.GetService<IMessageRegistry>();
            ISubscriptionHandler subscriptionHandler = provider.GetService<IntegrationEventProjectionToEventStore>();
            
            return new EventStoreBackgroundService
            (
                logger, eventStoreClient, provider, messageRegistry,
                new SubscriptionConfig
                (
                    "IntegrationEventsPub", 
                    subscriptionHandler,
                    // Subscription will be made to these streams:
                    // * process manager outbox and
                    // * aggregate
                    new SubscriptionFilterOptions(StreamFilter.RegularExpression
                        (new Regex($"^{processManagerStreamPrefix}.*outbox$|^{aggregateStreamPrefix}")))
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
            
            return new EventStoreBackgroundService
            (
                logger, eventStoreClient, provider, messageRegistry,
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