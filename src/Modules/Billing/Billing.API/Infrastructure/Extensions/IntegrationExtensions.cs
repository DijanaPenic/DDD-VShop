using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Billing.API.Application;
using VShop.Modules.Billing.Integration.Services;
using VShop.SharedKernel.Integration.Services;
using VShop.SharedKernel.EventStoreDb.Repositories;
using VShop.SharedKernel.EventSourcing.Repositories; // TODO - should move this class to XY and rename

namespace VShop.Modules.Billing.API.Infrastructure.Extensions
{
    public static class IntegrationExtensions
    {
        public static void AddIntegrationServices(this IServiceCollection services, string connectionString)
        {
            services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService>();
            services.AddTransient<IBillingIntegrationEventService, BillingIntegrationEventService>();

            EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(connectionString);
            eventStoreSettings.ConnectionName = "Billing";
            
            services.AddSingleton(_ => new EventStoreClient(eventStoreSettings));
            services.AddSingleton(typeof(IIntegrationRepository), typeof(EventStoreIntegrationRepository));
            
            MessageMappings.MapMessageTypes();
        }
    }
}