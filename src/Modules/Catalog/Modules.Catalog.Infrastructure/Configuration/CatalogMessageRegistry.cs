using VShop.Modules.Sales.Integration.Events;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.Modules.Catalog.Infrastructure.Configuration;

internal static class CatalogMessageRegistry
{
    public static IMessageRegistry Initialize()
    {
        MessageRegistry registry = new();

        registry.RegisterMessages();
        registry.RegisterTransformations();

        return registry;
    }

    private static void RegisterMessages(this MessageRegistry registry)
    {
        // Configure integration events - remote
        registry.Add<OrderStatusSetToPaidIntegrationEvent>(nameof(OrderStatusSetToPaidIntegrationEvent));
    }

    private static void RegisterTransformations(this MessageRegistry registry)
    {

    }
}