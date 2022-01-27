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
        
    }

    private static void RegisterTransformations(this MessageRegistry registry)
    {

    }
}