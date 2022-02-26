using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("VShop.Bootstrapper")]
namespace VShop.Modules.Identity.Infrastructure.Configuration;

internal static class IdentityCompositionRoot
{
    internal static string NamePrefix { get; private set; }
    internal static IServiceProvider ServiceProvider { get; private set; }

    internal static void SetServiceProvider(IServiceProvider serviceProvider, string namePrefix)
    {
        ServiceProvider = serviceProvider;
        NamePrefix = namePrefix;
    }

    internal static IServiceScope CreateScope()
        => ServiceProvider?.CreateScope();
}