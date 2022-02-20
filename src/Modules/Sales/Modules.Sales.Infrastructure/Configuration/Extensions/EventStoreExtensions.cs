using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Integration.Stores;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.EventSourcing.Stores;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Infrastructure.DAL;
using VShop.Modules.Sales.Infrastructure.Projections;
using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Integration.Extensions;

namespace VShop.Modules.Sales.Infrastructure.Configuration.Extensions;

internal static class EventStoreExtensions
{
    public static void AddEventStore(this IServiceCollection services, string connectionString)
    {
        services.AddEventStoreInfrastructure(connectionString, "Sales");

        services.AddTransient
        (
            typeof(IAggregateStore<>),
            typeof(AggregateStore<>)
        );
        services.AddTransient
        (
            typeof(IIntegrationEventStore),
            typeof(IntegrationEventStore)
        );

        // Stream names
        const string aggregateStreamPrefix = "sales/aggregate";

        // Read model projections
        services.AddReadModelBackgroundService<SalesDbContext>
        (
            "ShoppingCartReadModels",
            ShoppingCartInfoProjection.ProjectAsync,
            aggregateStreamPrefix
        );

        // Publish integration events from the current bounded context
        services.AddIntegrationEventPubBackgroundService($"^{aggregateStreamPrefix}");

        // Subscribe to integration streams
        services.AddIntegrationEventSubBackgroundService();
    }
}