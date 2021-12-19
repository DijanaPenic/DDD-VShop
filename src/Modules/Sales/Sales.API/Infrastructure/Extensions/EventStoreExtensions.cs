using Serilog;
using EventStore.Client;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.API.Projections;
using VShop.Modules.Sales.API.Application;
using VShop.SharedKernel.Application.Projections;
using VShop.SharedKernel.EventSourcing.Stores;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Integration.Stores;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Integration.Projections;
using VShop.SharedKernel.EventStoreDb.Subscriptions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Services;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;

namespace VShop.Modules.Sales.API.Infrastructure.Extensions
{
    public static class EventStoreExtensions
    {
        public static void AddEventStoreServices(this IServiceCollection services, string connectionString)
        {
            EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(connectionString);
            eventStoreSettings.ConnectionName = "Sales";
            
            EventStoreClient eventStoreClient = new(eventStoreSettings);
            services.AddSingleton(eventStoreClient);

            services.AddSingleton
            (
                typeof(IAggregateStore<>),
                typeof(AggregateStore<>)
            );
            services.AddSingleton
            (typeof(IIntegrationEventStore),
                typeof(IntegrationEventStore)
            );
            services.AddSingleton
            (typeof(IProcessManagerStore<>),
                typeof(ProcessManagerStore<>)
            );

            // Stream names
            string aggregateStreamPrefix = $"{eventStoreClient.ConnectionName}/aggregate".ToSnakeCase();
            string processManagerStreamPrefix = $"{eventStoreClient.ConnectionName}/process_manager".ToSnakeCase();

            // NOTE: Cannot use AddHostedService to register individual workers of the same type.
            // Source: https://github.com/dotnet/runtime/issues/38751
            services.AddHostedService<SubscriptionHostedService>();

            // Read model projections
            services.AddSingleton<ISubscriptionBackgroundService, SubscriptionToAllBackgroundService>(provider =>
            {
                ILogger logger = provider.GetService<ILogger>();
                return new SubscriptionToAllBackgroundService
                (
                    logger,
                    eventStoreClient,
                    provider,
                    new SubscriptionConfig
                    (
                        "ReadModels",
                        new DomainEventProjectionToPostgres<SalesContext>
                        (
                            logger,
                            provider,
                            ShoppingCartInfoProjection.ProjectAsync
                        ),
                        new SubscriptionFilterOptions(StreamFilter.Prefix(aggregateStreamPrefix))
                    )
                );
            });

            // Publish integration events from the current bounded context
            services.AddSingleton<ISubscriptionBackgroundService, SubscriptionToAllBackgroundService>(provider =>
            {
                ILogger logger = provider.GetService<ILogger>();
                return new SubscriptionToAllBackgroundService
                (
                    logger,
                    eventStoreClient,
                    provider,
                    new SubscriptionConfig
                    (
                        "IntegrationEventsPub",
                        new IntegrationEventProjectionToEventStore
                        (
                            logger,
                            provider,
                            provider.GetRequiredService<IIntegrationEventStore>()
                        ),
                        // Subscription will be made to these streams:
                        // * process manager outbox and
                        // * aggregate
                        new SubscriptionFilterOptions(StreamFilter.RegularExpression
                            (new Regex($"^{processManagerStreamPrefix}.*outbox$|^{aggregateStreamPrefix}")))
                    )
                );
            });

            // Subscribe to integration streams
            services.AddSingleton<ISubscriptionBackgroundService, SubscriptionToAllBackgroundService>(provider =>
            {
                ILogger logger = provider.GetService<ILogger>();
                return new SubscriptionToAllBackgroundService
                (
                    logger,
                    eventStoreClient,
                    provider,
                    new SubscriptionConfig
                    (
                        "IntegrationEventsSub",
                        new IntegrationEventPublisher
                        (
                            logger,
                            provider,
                            provider.GetRequiredService<IEventBus>()
                        ),
                        new SubscriptionFilterOptions(StreamFilter.RegularExpression
                            (new Regex(@".*\/integration$")))
                    )
                );
            });

            MessageMappings.MapMessageTypes();
        }
    }
}