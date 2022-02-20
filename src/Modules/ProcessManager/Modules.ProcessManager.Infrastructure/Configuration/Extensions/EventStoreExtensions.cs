using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventSourcing.Stores;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Integration.Extensions;
using VShop.SharedKernel.Integration.Stores;
using VShop.SharedKernel.Integration.Stores.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Configuration.Extensions;

internal static class EventStoreExtensions
{
    public static void AddEventStore(this IServiceCollection services, string connectionString)
    {
        services.AddEventStoreInfrastructure(connectionString, "ProcessManager");

        services.AddTransient
        (
            typeof(IIntegrationEventStore),
            typeof(IntegrationEventStore)
        );
        services.AddTransient
        (
            typeof(IProcessManagerStore<>),
            typeof(ProcessManagerStore<>)
        );

        // Publish integration events from the current bounded context
        services.AddIntegrationEventPubBackgroundService("^process_manager.*outbox");

        // Subscribe to integration streams
        services.AddIntegrationEventSubBackgroundService();
    }
}