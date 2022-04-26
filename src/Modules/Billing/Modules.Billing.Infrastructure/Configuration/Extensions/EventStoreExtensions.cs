using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Integration.Extensions;
using VShop.SharedKernel.Integration.Stores;
using VShop.SharedKernel.Integration.Services;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.SharedKernel.Integration.Stores.Contracts;

namespace VShop.Modules.Billing.Infrastructure.Configuration.Extensions
{
    internal static class EventStoreExtensions
    {
        public static void AddEventStore(this IServiceCollection services, EventStoreOptions eventStoreOptions)
        {
            services.AddTransient<IIntegrationEventOutbox, IntegrationEventOutbox>();
            services.AddTransient<IIntegrationEventService, IntegrationEventService>();
            services.AddEventStoreInfrastructure(eventStoreOptions.ConnectionString, "Billing");

            services.AddSingleton
            (
                typeof(IIntegrationEventStore),
                typeof(IntegrationEventStore)
            );

            // Subscribe to integration streams.
            services.AddIntegrationEventSubBackgroundService();
        }
    }
}