using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStore;
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
                "Sales.API"
            );
            
            services.AddSingleton(esConnection);
            services.AddSingleton(typeof(IEventStoreAggregateRepository<,>), typeof(EventStoreAggregateRepository<,>));
            services.AddSingleton<IHostedService, EventStoreService>();
            
            services.AddSingleton(provider =>
            {
                SalesContext dbContext = provider.GetRequiredService<SalesContext>();
                const string esSubscriptionName = "subscriptionReadModels";
                
                return new EventStoreSubscriptionManager
                (
                    esConnection,
                    new EventStoreCheckpointRepository(esConnection, esSubscriptionName),
                    esSubscriptionName,
                    new PostgresDbProjection<SalesContext>(dbContext, ShoppingCartInfoProjection.ProjectAsync)
                );
            });
            
            EventMappings.MapEventTypes();
        }
    }
}