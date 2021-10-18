using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStore;
using VShop.Services.Basket.Infrastructure;
using VShop.Services.Basket.API.Projections;

namespace VShop.Services.Basket.API.Infrastructure.Extensions
{
    public static class EventStoreExtensions
    {
        public static void AddEventStoreServices(this IServiceCollection services, string connectionString)
        {
            IEventStoreConnection esConnection = EventStoreConnection.Create
            (
                connectionString,
                ConnectionSettings.Create().KeepReconnecting().DisableTls(),
                "Basket.API"
            );
            services.AddSingleton(esConnection);
            services.AddSingleton(typeof(IEventStoreAggregateRepository<,>), typeof(EventStoreAggregateRepository<,>));
            services.AddSingleton<IHostedService, EventStoreService>();
            
            services.AddSingleton(provider =>
            {
                BasketContext dbContext = provider.GetRequiredService<BasketContext>();
                const string esSubscriptionName = "subscriptionReadModels";
                
                return new EventStoreSubscriptionManager
                (
                    esConnection,
                    new EventStoreCheckpointRepository(esConnection, esSubscriptionName),
                    esSubscriptionName,
                    new PostgresDbProjection<BasketContext>(dbContext, BasketDetailsProjection.ProjectAsync)
                );
            });
            
            EventMappings.MapEventTypes();
        }
    }
}