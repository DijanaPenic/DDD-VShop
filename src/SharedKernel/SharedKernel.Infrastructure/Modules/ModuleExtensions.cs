using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace VShop.SharedKernel.Infrastructure.Modules;

public static class ModuleExtensions
{
    public static IHostBuilder ConfigureModules(this IHostBuilder builder)
        => builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            foreach (string settings in GetSettings("*"))
            {
                cfg.AddJsonFile(settings);
            }

            foreach (string settings in GetSettings($"*.{ctx.HostingEnvironment.EnvironmentName}"))
            {
                cfg.AddJsonFile(settings);
            }

            IEnumerable<string> GetSettings(string pattern)
                => Directory.EnumerateFiles
                (
                    ctx.HostingEnvironment.ContentRootPath,
                    $"module.{pattern}.json",
                    SearchOption.AllDirectories
                );
        });
}