using System;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;

namespace VShop.Modules.Catalog.API
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();
            
            Log.Information("Starting up!");

            try
            {
                IHost host = CreateHostBuilder(args).Build();

                RunDatabaseMigrations(host);
                
                host.Run();

                Log.Information("Stopped cleanly");
                
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occured during host startup");
                
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    IHostEnvironment env = context.HostingEnvironment;
                
                    config.AddJsonFile("appsettings.json", true, true);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
                })
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                )
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        
        private static void RunDatabaseMigrations(IHost host)
        {
            using IServiceScope scope = host.Services
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            IServiceProvider serviceProvider = scope.ServiceProvider;
            
            // using CatalogContext catalogContext = serviceProvider.GetService<CatalogContext>();
            // catalogContext?.Database.Migrate();
        }
    }
}