using Serilog;
using Force.DeepCloner;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Tests.IntegrationTests.Probing;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure
{
    internal static class IntegrationTestsFixture
    {
        private static readonly IConfiguration Configuration;

        public static IMessageRegistry MessageRegistry => SalesMessageRegistry.Initialize();
        public static string EventStoreDbConnectionString => Configuration["EventStore:ConnectionString"];
        public static string RelationalDbConnectionString => Configuration["Sales:Postgres:ConnectionString"];

        static IntegrationTestsFixture()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("module.sales.tests.json")
                .Build();

            IContextAccessor contextAccessor = new MockContextAccessor();
            IModule module = ModuleLoader.LoadModules(Configuration).Single();
            ILogger logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .CreateLogger();
            
            module.ConfigureCompositionRoot(Configuration, logger, contextAccessor);
            
            InitializePostgresDatabaseAsync().GetAwaiter().GetResult();
        }

        private static Task InitializePostgresDatabaseAsync()
            => ExecuteHostedServiceAsync<DatabaseInitializerHostedService>
                (hostedService => hostedService.StartAsync(CancellationToken.None));

        public static Task AssertEventuallyAsync(IClockService clockService, IProbe probe, int timeout) 
            => new Poller(clockService, timeout).CheckAsync(probe);

        public static Task ExecuteHostedServiceAsync(Func<IHostedService, Task> action, string hostedServiceName)
            => ExecuteScopeAsync(sp =>
            {
                IHostedService service = sp.GetServices<IHostedService>()
                    .First(s => s.GetType().Name == hostedServiceName);

                return action(service);
            });
        
        public static Task ExecuteHostedServiceAsync<TService>(Func<TService, Task> action)
            where TService : IHostedService
            => ExecuteScopeAsync(sp =>
            {
                TService service = sp.GetServices<IHostedService>().OfType<TService>().Single();

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

        public static Task<Result> SendAsync(ICommand command, IContext context = default)
            => ExecuteScopeAsync(sp =>
            {
                ICommandDispatcher commandDispatcher = sp.GetRequiredService<ICommandDispatcher>();
                return commandDispatcher.SendAsync(command);
            }, context);

        public static Task<Result<TResult>> SendAsync<TResult>(ICommand<TResult> command, IContext context = default)
            => ExecuteScopeAsync(sp =>
            {
                ICommandDispatcher commandDispatcher = sp.GetRequiredService<ICommandDispatcher>();
                return commandDispatcher.SendAsync(command);
            }, context);

        public static Task PublishAsync(IBaseEvent @event, IContext context = default)
            => ExecuteScopeAsync(sp =>
            {
                IEventDispatcher eventDispatcher = sp.GetRequiredService<IEventDispatcher>();
                return eventDispatcher.PublishAsync(@event, EventDispatchStrategy.SyncStopOnException);
            }, context);

        private static async Task ExecuteScopeAsync
        (
            Func<IServiceProvider, Task> action,
            IContext context = default
        )
        {
            using IServiceScope scope = SalesCompositionRoot.CreateScope();
            
            IContextAccessor contextAccessor = scope.ServiceProvider.GetRequiredService<IContextAccessor>();
            contextAccessor.Context = context.DeepClone();
            
            await action(scope.ServiceProvider).ConfigureAwait(false);
        }

        private static async Task<TResult> ExecuteScopeAsync<TResult>
        (
            Func<IServiceProvider, Task<TResult>> action,
            IContext context = default
        )
        {
            using IServiceScope scope = SalesCompositionRoot.CreateScope();
            
            IContextAccessor contextAccessor = scope.ServiceProvider.GetRequiredService<IContextAccessor>();
            contextAccessor.Context = context.DeepClone();
                                      
            return await action(scope.ServiceProvider).ConfigureAwait(false);
        }
    }
}