using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Modules.Contracts;

namespace VShop.SharedKernel.Infrastructure.Modules;

public static class ModuleExtensions
{
    public static IHostBuilder ConfigureModules(this IHostBuilder builder)
        => builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            cfg.AddEnvironmentVariables("VShopApp_");
            
            foreach (string settings in GetSettings("*"))
                cfg.AddJsonFile(settings);

            foreach (string settings in GetSettings($"*.{ctx.HostingEnvironment.EnvironmentName.ToLowerInvariant()}"))
                cfg.AddJsonFile(settings);

            IEnumerable<string> GetSettings(string pattern)
                => Directory.EnumerateFiles
                (
                    ctx.HostingEnvironment.ContentRootPath,
                    $"module.{pattern}.json",
                    SearchOption.AllDirectories
                );
        });
    
    public static IServiceCollection AddModuleRequests(this IServiceCollection services)
    {
        services.AddSingleton<IModuleClient, ModuleClient>();
        services.AddSingleton<IModuleSerializer, JsonModuleSerializer>();

        return services;
    }
}