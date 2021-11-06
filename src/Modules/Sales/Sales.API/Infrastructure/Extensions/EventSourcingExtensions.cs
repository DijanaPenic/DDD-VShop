﻿using EventStore.ClientAPI;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.EventStore.Projections;
using VShop.SharedKernel.EventStore.Repositories;
using VShop.SharedKernel.EventStore.Subscriptions;
using VShop.SharedKernel.EventStore.Subscriptions.Contracts;
using VShop.SharedKernel.PostgresDb.Projections;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging.Events.Publishing;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.API.Projections;
using VShop.Modules.Sales.API.Application;

namespace VShop.Modules.Sales.API.Infrastructure.Extensions
{
    public static class EventSourcingExtensions
    {
        public static void AddEventSourcingServices(this IServiceCollection services, string connectionString)
        {
            // TODO - switch to gRPC
            IEventStoreConnection eventStoreConnection = EventStoreConnection.Create
            (
                connectionString,
                ConnectionSettings.Create().KeepReconnecting().DisableTls(),
                "Sales"
            );

            services.AddSingleton(eventStoreConnection);
            services.AddSingleton(typeof(IAggregateRepository<,>), typeof(EventStoreAggregateRepository<,>));
            services.AddSingleton(typeof(IIntegrationRepository), typeof(EventStoreIntegrationRepository));
            services.AddSingleton(typeof(IProcessManagerRepository<>), typeof(EventStoreProcessManagerRepository<>));
            services.AddHostedService<EventStoreService>();
            
            // Read model projections
            services.AddSingleton<IEventStoreSubscriptionManager, EventStoreAllCatchUpSubscriptionManager>(provider =>
            {
                const string subscriptionName = "ReadModels";

                return new EventStoreAllCatchUpSubscriptionManager
                (
                    eventStoreConnection,
                    new EventStoreCheckpointRepository(eventStoreConnection, subscriptionName),
                    subscriptionName,
                    new DomainEventProjectionToPostgres<SalesContext>(provider, ShoppingCartInfoProjection.ProjectAsync)
                );
            });
            
            // Publish integration events from the current bounded context 
            services.AddSingleton<IEventStoreSubscriptionManager, EventStoreAllFilteredCatchUpSubscriptionManager>(provider =>
            {
                IIntegrationRepository integrationRepository = provider.GetRequiredService<IIntegrationRepository>();
                const string subscriptionName = "IntegrationEventsPub";

                return new EventStoreAllFilteredCatchUpSubscriptionManager
                (
                    eventStoreConnection,
                    new EventStoreCheckpointRepository(eventStoreConnection, subscriptionName),
                    subscriptionName,
                    Filter.StreamId.Prefix($"{eventStoreConnection.ConnectionName}/aggregate".ToSnakeCase()),
                new IntegrationEventProjectionToEventStore(integrationRepository)
                );
            });
            
            // Subscribe to all integration streams
            services.AddSingleton<IEventStoreSubscriptionManager, EventStoreAllFilteredCatchUpSubscriptionManager>(provider =>
            {
                Publisher publisher = provider.GetRequiredService<Publisher>();
                const string subscriptionName = "IntegrationEventsSub";

                return new EventStoreAllFilteredCatchUpSubscriptionManager
                (
                    eventStoreConnection,
                    new EventStoreCheckpointRepository(eventStoreConnection, subscriptionName),
                    subscriptionName,
                    Filter.StreamId.Regex(new Regex(@".*\/integration$")),
                    new IntegrationEventProjectionPublisher(publisher)
                );
            });

            MessageMappings.MapMessageTypes();
        }
    }
}