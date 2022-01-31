using Microsoft.Extensions.DependencyInjection;

namespace VShop.Modules.ProcessManager.Infrastructure.Configuration;

internal static class ProcessManagerCompositionRoot
{
    internal static string NamePrefix => "VShop.Modules.ProcessManager";
    internal static IServiceProvider ServiceProvider { get; private set; }

    internal static void SetServiceProvider(IServiceProvider serviceProvider) 
        => ServiceProvider = serviceProvider;

    internal static IServiceScope CreateScope()
        => ServiceProvider?.CreateScope();
}