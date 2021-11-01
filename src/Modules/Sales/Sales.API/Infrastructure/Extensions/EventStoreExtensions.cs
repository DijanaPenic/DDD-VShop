﻿using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.PostgresDb.Projections;
using VShop.SharedKernel.EventStore.Projections;
using VShop.SharedKernel.EventStore.Repositories;
using VShop.SharedKernel.EventStore.Repositories.Contracts;
using VShop.SharedKernel.EventStore.Subscriptions;
using VShop.SharedKernel.EventStore.Subscriptions.Contracts;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.API.Projections;

namespace VShop.Modules.Sales.API.Infrastructure.Extensions
{
    public static class EventStoreExtensions
    {
        public static void AddEventStoreServices(this IServiceCollection services, string connectionString)
        {
            IEventStoreConnection esConnection = EventStoreConnection.Create
            (
                connectionString,
                ConnectionSettings.Create().KeepReconnecting().DisableTls(),
                "Sales"
            );

            services.AddSingleton(esConnection);
            services.AddSingleton(typeof(IEventStoreAggregateRepository<,>), typeof(EventStoreAggregateRepository<,>));
            services.AddSingleton(typeof(IEventStoreIntegrationRepository), typeof(EventStoreIntegrationRepository));
            services.AddSingleton<IHostedService, EventStoreService>();
            
            services.AddSingleton<IEventStoreSubscriptionManager, EventStoreAllCatchUpSubscriptionManager>(provider =>
            {
                SalesContext dbContext = provider.GetRequiredService<SalesContext>();
                const string esSubscriptionName = "subscriptionReadModels";
                
                return new EventStoreAllCatchUpSubscriptionManager
                (
                    esConnection,
                    new EventStoreCheckpointRepository(esConnection, esSubscriptionName),
                    esSubscriptionName,
                    new PostgresDomainProjection<SalesContext>(dbContext, ShoppingCartInfoProjection.ProjectAsync)
                );
            });
            
            services.AddSingleton<IEventStoreSubscriptionManager, EventStoreAllCatchUpSubscriptionManager>(provider =>
            {
                IEventStoreIntegrationRepository integrationRepository = provider.GetRequiredService<IEventStoreIntegrationRepository>();
                const string esSubscriptionName = "subscriptionIntegrationEventsPub";
                
                return new EventStoreAllCatchUpSubscriptionManager
                (
                    esConnection,
                    new EventStoreCheckpointRepository(esConnection, esSubscriptionName),
                    esSubscriptionName,
                    new EventStoreIntegrationProjection(integrationRepository)
                );
            });
            
            EventMappings.MapEventTypes();
        }
    }
}