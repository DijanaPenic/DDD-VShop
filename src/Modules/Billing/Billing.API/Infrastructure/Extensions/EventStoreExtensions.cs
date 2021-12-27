using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Billing.API.Application;
using VShop.SharedKernel.Integration.Stores;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Integration.Services;
using VShop.SharedKernel.Integration.Services.Contracts;

namespace VShop.Modules.Billing.API.Infrastructure.Extensions
{
    public static class EventStoreExtensions
    {
        public static void AddEventStoreServices(this IServiceCollection services, string connectionString)
        {
            services.AddTransient<IIntegrationEventLogStore, IntegrationEventLogStore>();
            services.AddTransient<IIntegrationEventService, IntegrationEventService>();

            EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(connectionString);
            eventStoreSettings.ConnectionName = "Billing";
            
            services.AddSingleton(_ => new EventStoreClient(eventStoreSettings));
            
            services.AddSingleton
            (typeof(IIntegrationEventStore),
                typeof(IntegrationEventStore)
            );
            
            MessageMappings.MapMessageTypes();
        }
    }
}