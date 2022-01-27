using Moq;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Tests.IntegrationTests.Probing;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure
{
    internal static class IntegrationTestsFixture
    {
        private static readonly IConfiguration Configuration;

        public static IMessageRegistry MessageRegistry => SalesMessageRegistry.Initialize();
        public static string EventStoreDbConnectionString => Configuration.GetConnectionString("EventStoreDb");
        public static string RelationalDbConnectionString => Configuration.GetConnectionString("PostgresDb");

        static IntegrationTestsFixture()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("module.sales.tests.json")
                .Build();

            IModule module = ModuleLoader.LoadModules(Configuration, "VShop.Modules.Sales").Single();
            ILogger logger = new Mock<ILogger>().Object;
            
            module.Initialize(Configuration, logger);
        }

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
                ICommandDispatcher commandDispatcher = sp.GetRequiredService<ICommandDispatcher>();
                return commandDispatcher.SendAsync(command);
            });

        public static Task<Result<TResult>> SendAsync<TResult>(ICommand<TResult> command)
            => ExecuteScopeAsync(sp =>
            {
                ICommandDispatcher commandDispatcher = sp.GetRequiredService<ICommandDispatcher>();
                return commandDispatcher.SendAsync(command);
            });
        
        public static Task PublishAsync(IDomainEvent @event)
            => ExecuteScopeAsync(sp =>
            {
                IEventDispatcher eventDispatcher = sp.GetRequiredService<IEventDispatcher>();
                return eventDispatcher.PublishAsync(@event);
            });
        
        public static Task PublishAsync(IIntegrationEvent @event)
            => ExecuteScopeAsync(sp =>
            {
                IEventDispatcher eventDispatcher = sp.GetRequiredService<IEventDispatcher>();
                return eventDispatcher.PublishAsync(@event);
            });

        private static async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using IServiceScope scope = SalesCompositionRoot.CreateScope();
            await action(scope.ServiceProvider).ConfigureAwait(false);
        }
        
        private static async Task<TResult> ExecuteScopeAsync<TResult>(Func<IServiceProvider, Task<TResult>> action)
        {
            using IServiceScope scope = SalesCompositionRoot.CreateScope();
            return await action(scope.ServiceProvider).ConfigureAwait(false);
        }
    }
}