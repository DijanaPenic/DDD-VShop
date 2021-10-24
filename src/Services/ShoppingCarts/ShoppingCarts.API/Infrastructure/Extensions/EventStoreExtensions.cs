using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStore;
using VShop.Services.ShoppingCarts.Infrastructure;
using VShop.Services.ShoppingCarts.API.Projections;

namespace VShop.Services.ShoppingCarts.API.Infrastructure.Extensions
{
    public static class EventStoreExtensions
    {
        public static void AddEventStoreServices(this IServiceCollection services, string connectionString)
        {
            IEventStoreConnection esConnection = EventStoreConnection.Create
            (
                connectionString,
                ConnectionSettings.Create().KeepReconnecting().DisableTls(),
                "ShoppingCarts.API"
            );
            
            services.AddSingleton(esConnection);
            services.AddSingleton(typeof(IEventStoreAggregateRepository<,>), typeof(EventStoreAggregateRepository<,>));
            services.AddSingleton<IHostedService, EventStoreService>();
            
            services.AddSingleton(provider =>
            {
                ShoppingCartContext dbContext = provider.GetRequiredService<ShoppingCartContext>();
                const string esSubscriptionName = "subscriptionReadModels";
                
                return new EventStoreSubscriptionManager
                (
                    esConnection,
                    new EventStoreCheckpointRepository(esConnection, esSubscriptionName),
                    esSubscriptionName,
                    new PostgresDbProjection<ShoppingCartContext>(dbContext, ShoppingCartInfoProjection.ProjectAsync)
                );
            });
            
            EventMappings.MapEventTypes();
        }
    }
}