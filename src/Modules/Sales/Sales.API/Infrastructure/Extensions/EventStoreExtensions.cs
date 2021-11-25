﻿using EventStore.Client;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;


using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.API.Projections;
using VShop.Modules.Sales.API.Application;
using VShop.SharedKernel.Integration.Projections;
using VShop.SharedKernel.Integration.Repositories;
using VShop.SharedKernel.Integration.Repositories.Contracts;
using VShop.SharedKernel.EventSourcing.Projections;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.SharedKernel.EventStoreDb.Subscriptions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Services;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Services.Contracts;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Checkpoints;
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
            
            services.AddSingleton(typeof(IAggregateRepository<,>), typeof(EventStoreAggregateRepository<,>));
            services.AddSingleton(typeof(IIntegrationRepository), typeof(EventStoreIntegrationRepository));
            services.AddSingleton(typeof(IProcessManagerRepository<>), typeof(EventStoreProcessManagerRepository<>));

            // Stream names
            string aggregateStreamPrefix = $"{eventStoreClient.ConnectionName}/aggregate".ToSnakeCase();
            string processManagerStreamPrefix = $"{eventStoreClient.ConnectionName}/process_manager".ToSnakeCase();

            // NOTE: Cannot use AddHostedService to register multiple workers: https://github.com/dotnet/runtime/issues/38751
            services.AddHostedService<SubscriptionHostedService>();
            
            // Read model projections
            services.AddSingleton<ISubscriptionBackgroundService, SubscriptionToAllBackgroundService>(provider => new SubscriptionToAllBackgroundService
            (
                eventStoreClient,
                new EventStoreCheckpointRepository(eventStoreClient), // TODO - implement PostgresDb checkpoint (needed for atomic operation)
                "ReadModels",
                new ISubscriptionHandler[]
                {
                    new DomainEventProjectionToPostgres<SalesContext>(provider, ShoppingCartInfoProjection.ProjectAsync)
                },
                new SubscriptionFilterOptions(StreamFilter.Prefix(aggregateStreamPrefix))
            ));

            // Publish integration events from the current bounded context
            services.AddSingleton<ISubscriptionBackgroundService, SubscriptionToAllBackgroundService>(provider => new SubscriptionToAllBackgroundService
            (
                eventStoreClient,
                new EventStoreCheckpointRepository(eventStoreClient),
                "IntegrationEventsPub",
                new ISubscriptionHandler[]
                {
                    new IntegrationEventProjectionToEventStore(provider.GetRequiredService<IIntegrationRepository>())
                },
                // This will subscribe to these streams:
                // * process manager outbox and
                // * aggregate
                new SubscriptionFilterOptions(StreamFilter.RegularExpression(new Regex($"^{processManagerStreamPrefix}.*outbox$|^{aggregateStreamPrefix}")))
            ));

            // Subscribe to all integration streams
            services.AddSingleton<ISubscriptionBackgroundService, SubscriptionToAllBackgroundService>(provider => new SubscriptionToAllBackgroundService
            (
                eventStoreClient,
                new EventStoreCheckpointRepository(eventStoreClient),
                "IntegrationEventsSub",
                new ISubscriptionHandler[]
                {
                    new IntegrationEventPublisher(provider.GetRequiredService<IEventBus>())
                },
                new SubscriptionFilterOptions(StreamFilter.RegularExpression(new Regex(@".*\/integration$")))
            ));

            MessageMappings.MapMessageTypes();
        }
    }
}