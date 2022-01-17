using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Autofac.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Scheduler.Infrastructure;
using VShop.SharedKernel.Tests.IntegrationTests.Probing;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;
using VShop.Modules.Sales.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure
{
    public static class IntegrationTestsFixture
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

            ServiceScopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
            Configuration = host.Services.GetRequiredService<IConfiguration>();

            RunRelationalDatabaseMigrationsAsync().GetAwaiter();
        }

        public static string EventStoreDbConnectionString => Configuration.GetConnectionString("EventStoreDb");
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
        
        public static Task ExecuteHostedServiceAsync<TService>(Func<TService, Task> action)
            where TService : IHostedService
            => ExecuteScopeAsync(sp =>
            {
                TService service = sp.GetServices<IHostedService>().OfType<TService>().SingleOrDefault();

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

        public static Task<Result> SendAsync(ICommand command)
            => ExecuteScopeAsync(sp =>
            {
                ICommandBus commandBus = sp.GetRequiredService<ICommandBus>();
                return commandBus.SendAsync(command);
            });

        public static Task<Result<TResult>> SendAsync<TResult>(ICommand<TResult> command)
            => ExecuteScopeAsync(sp =>
            {
                ICommandBus commandBus = sp.GetRequiredService<ICommandBus>();
                return commandBus.SendAsync(command);
            });
        
        public static Task PublishAsync(IDomainEvent @event)
            => ExecuteScopeAsync(sp =>
            {
                IEventBus eventBus = sp.GetRequiredService<IEventBus>();
                return eventBus.Publish(@event);
            });
        
        public static Task PublishAsync(IIntegrationEvent @event)
            => ExecuteScopeAsync(sp =>
            {
                IEventBus eventBus = sp.GetRequiredService<IEventBus>();
                return eventBus.Publish(@event);
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
        
        private static async Task RunRelationalDatabaseMigrationsAsync()
        { 
            await MigratePostgresDatabaseAsync<SalesDbContext>();
            await MigratePostgresDatabaseAsync<SchedulerDbContext>();
            await MigratePostgresDatabaseAsync<SubscriptionDbContext>();
        }
        
        private static Task MigratePostgresDatabaseAsync<TDbContext>() 
            where TDbContext : DbContextBase
            => ExecuteServiceAsync<TDbContext>(dbContext => dbContext.Database.MigrateAsync());
    }
}