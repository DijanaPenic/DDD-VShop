using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Bootstrapper")]
namespace VShop.Modules.Billing.Infrastructure.Configuration;

internal class BillingCompositionRoot
{
    internal static string NamePrefix => "VShop.Modules.Billing";
    internal static IServiceProvider ServiceProvider;

    internal static void SetServiceProvider(IServiceProvider serviceProvider) 
        => ServiceProvider = serviceProvider;

    internal static IServiceScope CreateScope()
        => ServiceProvider.CreateScope();
}

