using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Tests.IntegrationTests.Probing;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure
{
    public static class IntegrationTestsFixture // TODO - potentially use as collection fixture
    {
        private static readonly IServiceScopeFactory ServiceScopeFactory;
        private static readonly IConfiguration Configuration;

        static IntegrationTestsFixture()
        {
            IHost host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((_, config) =>
                {
                    config.AddJsonFile("appsettings.Test.json");
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
                })
                .Build();
            
            //host.Start(); // TODO - problem with this, EventStore database is not clean so all events are picked up by subscriptions

            ServiceScopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
            Configuration = host.Services.GetRequiredService<IConfiguration>();
        }

        public static string EventStoreDbConnectionString => Configuration.GetConnectionString("EventStoreDb");
        public static string EventStorePortalUrl => Configuration.GetConnectionString("EventStoreDbPortalUrl");
        public static string RelationalDbConnectionString => Configuration.GetConnectionString("PostgresDb");
        
        public static Task AssertEventuallyAsync(IClockService clockService, IProbe probe, int timeout) 
            => new Poller(clockService, timeout).CheckAsync(probe);

        public static Task ExecuteHostedServiceAsync(Func<IHostedService, Task> action, string hostedServiceName)
            => ExecuteScopeAsync(sp =>
            {
                IHostedService service = sp.GetServices<IHostedService>()
                    .FirstOrDefault(s => s.GetType().Name == hostedServiceName);

                return action(service);
            });

        public static Task ExecuteServiceAsync<TService>(Func<TService, Task> action)
            => ExecuteScopeAsync(sp =>
            {
                TService service = sp.GetRequiredService<TService>();
                return action(service);
            });
        
        public static Task<TResult> ExecuteServiceAsync<TService, TResult>(Func<TService, Task<TResult>> action)
            => ExecuteScopeAsync(sp =>
            {
                TService service = sp.GetRequiredService<TService>();
                return action(service);
            });

        public static Task<TResult> SendAsync<TResult>(IRequest<TResult> command)
            => ExecuteScopeAsync(sp =>
            {
                ICommandBus commandBus = sp.GetRequiredService<ICommandBus>();
                return commandBus.SendAsync(command);
            });

        private static async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using IServiceScope scope = ServiceScopeFactory.CreateScope();
            await action(scope.ServiceProvider).ConfigureAwait(false);
        }
        
        private static async Task<TResult> ExecuteScopeAsync<TResult>(Func<IServiceProvider, Task<TResult>> action)
        {
            using IServiceScope scope = ServiceScopeFactory.CreateScope();
            return await action(scope.ServiceProvider).ConfigureAwait(false);
        }
    }
}