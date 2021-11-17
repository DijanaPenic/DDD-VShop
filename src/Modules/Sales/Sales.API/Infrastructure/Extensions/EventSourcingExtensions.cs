using EventStore.Client;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.EventStore.Projections;
using VShop.SharedKernel.EventStore.Repositories;
using VShop.SharedKernel.EventStore.Subscriptions;
using VShop.SharedKernel.PostgresDb.Projections;
using VShop.SharedKernel.EventSourcing.Projections;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging.Events.Publishing;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.API.Projections;
using VShop.Modules.Sales.API.Application;

namespace VShop.Modules.Sales.API.Infrastructure.Extensions
{
    public static class EventSourcingExtensions
    {
        public static void AddEventSourcingServices(this IServiceCollection services, string connectionString)
        {
            EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(connectionString);
            eventStoreSettings.ConnectionName = "Sales";
            
            EventStoreClient eventStoreClient = new(eventStoreSettings);
            services.AddSingleton(eventStoreClient);
            
            services.AddSingleton(typeof(IAggregateRepository<,>), typeof(EventStoreAggregateRepository<,>));
            services.AddSingleton(typeof(IIntegrationRepository), typeof(EventStoreIntegrationRepository));
            services.AddSingleton(typeof(IProcessManagerRepository<>), typeof(EventStoreProcessManagerRepository<>));

            // Stream names
            string aggregateStreamPrefix = $"{eventStoreClient.ConnectionName}/aggregate".ToSnakeCase();
            string processManagerStreamPrefix = $"{eventStoreClient.ConnectionName}/process_manager".ToSnakeCase();

            // NOTE: Cannot use AddHostedService to register multiple workers: https://github.com/dotnet/runtime/issues/38751
            services.AddHostedService<EventStoreHostedService>();
            
            // TODO - uncomment
            // Read model projections
            // services.AddSingleton<ISubscribeBackgroundService, SubscribeToAllBackgroundService>(provider => new SubscribeToAllBackgroundService
            // (
            //     eventStoreClient,
            //     new EventStoreCheckpointRepository(eventStoreClient), // TODO - implement PostgresDb checkpoint (needed for atomic operation)
            //     "ReadModels",
            //     new ISubscription[]
            //     {
            //         new DomainEventProjectionToPostgres<SalesContext>(provider, ShoppingCartInfoProjection.ProjectAsync)
            //     },
            //     new SubscriptionFilterOptions(StreamFilter.Prefix(aggregateStreamPrefix))
            // ));

            // Publish integration events from the current bounded context
            services.AddSingleton<ISubscribeBackgroundService, SubscribeToAllBackgroundService>(provider => new SubscribeToAllBackgroundService
            (
                eventStoreClient,
                new EventStoreCheckpointRepository(eventStoreClient),
                "IntegrationEventsPub",
                new ISubscription[]
                {
                    new IntegrationEventProjectionToEventStore(provider.GetRequiredService<IIntegrationRepository>())
                },
                // This will subscribe to these streams:
                // * process manager outbox and
                // * aggregate
                new SubscriptionFilterOptions(StreamFilter.RegularExpression(new Regex($"^{processManagerStreamPrefix}.*outbox$|^{aggregateStreamPrefix}")))
            ));

            // Subscribe to all integration streams
            services.AddSingleton<ISubscribeBackgroundService, SubscribeToAllBackgroundService>(provider => new SubscribeToAllBackgroundService
            (
                eventStoreClient,
                new EventStoreCheckpointRepository(eventStoreClient),
                "IntegrationEventsSub",
                new ISubscription[]
                {
                    new IntegrationEventProjectionPublisher(provider.GetRequiredService<EventBus>())
                },
                new SubscriptionFilterOptions(StreamFilter.RegularExpression(new Regex(@".*\/integration$")))
            ));

            MessageMappings.MapMessageTypes();
        }
    }
}