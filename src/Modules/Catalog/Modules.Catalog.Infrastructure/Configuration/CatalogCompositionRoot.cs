using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("VShop.Bootstrapper")]
[assembly: InternalsVisibleTo("VShop.Tests.IntegrationTests")]
namespace VShop.Modules.Catalog.Infrastructure.Configuration;

internal static class CatalogCompositionRoot
{
    internal static string NamePrefix => "VShop.Modules.Catalog";
    internal static IServiceProvider ServiceProvider { get; private set; }

    internal static void SetServiceProvider(IServiceProvider serviceProvider) 
        => ServiceProvider = serviceProvider;

    internal static IServiceScope CreateScope()
        => ServiceProvider?.CreateScope();
}