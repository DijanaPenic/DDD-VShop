using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventStore;
using VShop.Services.Basket.Infrastructure;

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
            services.AddSingleton(typeof(IEventStoreRepository<,>), typeof(EventStoreRepository<,>));
            services.AddSingleton<IHostedService, EventStoreService>();
            
            EventMappings.MapEventTypes();
        }
    }
}