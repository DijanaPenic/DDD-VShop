using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

// TODO - VShop prefix for Bootstrapper.
[assembly: InternalsVisibleTo("Bootstrapper")]
[assembly: InternalsVisibleTo("VShop.Tests.IntegrationTests")]
namespace VShop.Modules.Sales.Infrastructure.Configuration;

internal static class SalesCompositionRoot
{
    internal static string NamePrefix => "VShop.Modules.Sales";
    internal static IServiceProvider ServiceProvider { get; private set; }

    internal static void SetServiceProvider(IServiceProvider serviceProvider) 
        => ServiceProvider = serviceProvider;

    internal static IServiceScope CreateScope()
        => ServiceProvider?.CreateScope();
}