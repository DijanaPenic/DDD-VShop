using Serilog;
using EventStore.Client;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventSourcing.Stores;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventStoreDb.Messaging.Contracts;
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
        services.AddEventStoreInfrastructure(connectionString, "ProcessManager");

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

        // Publish integration events from the current bounded context
        services.AddSingleton<IntegrationEventProjectionToEventStore>();
        services.AddSingleton<IEventStoreBackgroundService, EventStoreBackgroundService>(provider =>
        {
            EventStoreClient eventStoreClient = provider.GetService<EventStoreClient>();
            ILogger logger = provider.GetService<ILogger>();
            IMessageRegistry messageRegistry = provider.GetService<IMessageRegistry>();
            IEventStoreMessageConverter eventStoreMessageConverter = provider.GetService<IEventStoreMessageConverter>();
            ISubscriptionHandler subscriptionHandler = provider.GetService<IntegrationEventProjectionToEventStore>();
            IMessageContextRegistry messageContextRegistry = provider.GetService<IMessageContextRegistry>();

            return new EventStoreBackgroundService
            (
                logger, eventStoreClient, provider, messageRegistry, 
                eventStoreMessageConverter, messageContextRegistry,
                new SubscriptionConfig
                (
                    "IntegrationEventsPub", 
                    subscriptionHandler,
                    new SubscriptionFilterOptions(StreamFilter.RegularExpression
                        (new Regex("^process_manager.*outbox")))
                )
            );
        });

        // Subscribe to integration streams
        services.AddSingleton<IntegrationEventPublisher>();
        services.AddSingleton<IEventStoreBackgroundService, EventStoreBackgroundService>(provider =>
        {
            EventStoreClient eventStoreClient = provider.GetService<EventStoreClient>();
            ILogger logger = provider.GetService<ILogger>();
            IEventStoreMessageConverter eventStoreMessageConverter = provider.GetService<IEventStoreMessageConverter>();
            IMessageRegistry messageRegistry = provider.GetService<IMessageRegistry>();
            ISubscriptionHandler subscriptionHandler = provider.GetService<IntegrationEventPublisher>();
            IMessageContextRegistry messageContextRegistry = provider.GetService<IMessageContextRegistry>();

            return new EventStoreBackgroundService
            (
                logger, eventStoreClient, provider, messageRegistry,
                eventStoreMessageConverter, messageContextRegistry,
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