using System;
using System.IO;
using System.Collections.Generic;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
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
            foreach (string settings in GetSettings("*"))
                cfg.AddJsonFile(settings);

            string environment = ctx.HostingEnvironment.EnvironmentName.ToLowerInvariant();
            foreach (string settings in GetSettings($"*.{environment}"))
                cfg.AddJsonFile(settings);

            IEnumerable<string> GetSettings(string pattern)
                => Directory.EnumerateFiles
                (
                    ctx.HostingEnvironment.ContentRootPath,
                    $"module.{pattern}.json",
                    SearchOption.AllDirectories
                );
            
            IConfigurationRoot configuration = cfg.Build();

            if (!ctx.HostingEnvironment.IsDevelopment())
            {
                SecretClient secretClient = new
                (
                    new Uri($"https://{configuration["KeyVault"]}.vault.azure.net/"),
                    new DefaultAzureCredential()
                );
                cfg.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
            }
            else cfg.AddEnvironmentVariables($"{configuration["App:Name"]}App_");
        });
    
    public static IServiceCollection AddModuleRequests(this IServiceCollection services)
    {
        services.AddSingleton<IModuleClient, ModuleClient>();
        services.AddSingleton<IModuleSerializer, JsonModuleSerializer>();

        return services;
    }
}