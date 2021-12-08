using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Billing.Infrastructure;
using VShop.Modules.Billing.API.Application;
using VShop.SharedKernel.Integration.Services;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.SharedKernel.Integration.Repositories;
using VShop.SharedKernel.Integration.Repositories.Contracts;

namespace VShop.Modules.Billing.API.Infrastructure.Extensions
{
    public static class IntegrationExtensions
    {
        public static void AddIntegrationServices(this IServiceCollection services, string connectionString)
        {
            services.AddTransient<IIntegrationEventLogRepository, IntegrationEventLogRepository>();
            services.AddTransient<IIntegrationEventService, IntegrationEventService<BillingContext>>();

            EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(connectionString);
            eventStoreSettings.ConnectionName = "Billing";
            
            services.AddSingleton(_ => new EventStoreClient(eventStoreSettings));
            services.AddSingleton(typeof(IIntegrationEventRepository), typeof(IntegrationEventRepository));
            
            MessageMappings.MapMessageTypes();
        }
    }
}