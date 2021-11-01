using EventStore.ClientAPI;
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
using VShop.SharedKernel.EventStore.Subscriptions.Settings;

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

            services.AddSingleton(provider =>
            {
                SalesContext dbContext = provider.GetRequiredService<SalesContext>();
                const string esSubscriptionName = "ReadModels";

                return new EventStoreAllCatchUpSubscriptionManager
                (
                    esConnection,
                    esSubscriptionName,
                    new PostgresDomainProjection<SalesContext>(dbContext, ShoppingCartInfoProjection.ProjectAsync)
                );
            });
            
            services.AddSingleton(provider =>
            {
                IEventStoreIntegrationRepository integrationRepository = provider.GetRequiredService<IEventStoreIntegrationRepository>();
                const string esSubscriptionName = "IntegrationEventsPub";
                
                return new EventStoreAllCatchUpSubscriptionManager
                (
                    esConnection,
                    esSubscriptionName,
                new EventStoreIntegrationProjection(integrationRepository)
                );
            });
            
            EventMappings.MapEventTypes();
        }
    }
}