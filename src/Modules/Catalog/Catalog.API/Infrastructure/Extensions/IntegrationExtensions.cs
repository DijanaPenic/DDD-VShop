using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Integration.Stores;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Integration.Services;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Catalog.API.Application;

namespace VShop.Modules.Catalog.API.Infrastructure.Extensions
{
    public static class IntegrationExtensions
    {
        public static void AddIntegrationServices(this IServiceCollection services, string connectionString)
        {
            services.AddTransient<IIntegrationEventLogStore, IntegrationEventLogStore>();
            services.AddTransient<IIntegrationEventService, IntegrationEventService>();

            EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(connectionString);
            eventStoreSettings.ConnectionName = "Product";
            
            services.AddSingleton(_ => new EventStoreClient(eventStoreSettings));
            services.AddSingleton(typeof(IIntegrationEventStore), typeof(IntegrationEventStore));
            
            MessageMappings.MapMessageTypes();
        }
    }
}