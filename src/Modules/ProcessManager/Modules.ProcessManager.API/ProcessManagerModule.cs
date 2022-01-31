using MediatR;
using Serilog;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using VShop.Modules.ProcessManager.Infrastructure;
using VShop.Modules.ProcessManager.Infrastructure.Configuration;
using VShop.Modules.ProcessManager.Infrastructure.Configuration.Extensions;
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;
using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Services.Contracts;

namespace VShop.Modules.ProcessManager.API;

internal class ProcessManagerModule : IModule
{
    public string Name => "ProcessManager";
    public Assembly[] Assemblies { get; set; }

    public void Initialize
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor
    )
    {
        ConfigureCompositionRoot(configuration, logger, contextAccessor);

        ModuleRegistry.AddBroadcastActions(ProcessManagerCompositionRoot.ServiceProvider, Assemblies);
        
        IEnumerable<IEventStoreBackgroundService> subscriptionServices = ProcessManagerCompositionRoot.ServiceProvider
            .GetServices<IEventStoreBackgroundService>();
        ModuleEventStoreSubscriptionRegistry.Add(subscriptionServices);
        
        RunHostedServices();
    }

    public void ConfigureCompositionRoot
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor
    )
    {
        PostgresOptions postgresOptions = configuration
            .GetOptions<PostgresOptions>($"{Name}:Postgres");
        EventStoreOptions eventStoreOptions = configuration
            .GetOptions<EventStoreOptions>("EventStore");
        
        ServiceCollection services = new();
        
        services.AddInfrastructure(Assemblies, Name, logger, contextAccessor);
        services.AddPostgres(postgresOptions.ConnectionString);
        services.AddScheduler(postgresOptions.ConnectionString);
        services.AddEventStore(eventStoreOptions.ConnectionString);
        services.AddSingleton(ProcessManagerMessageRegistry.Initialize());
        services.AddSingleton<IProcessManagerDispatcher, ProcessManagerDispatcher>();
        services.AddSingleton<IDispatcher, ProcessManagerDispatcher>();
        
        services.Decorate
        (
            typeof(INotificationHandler<>),
            typeof(LoggingEventDecorator<>)
        );

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        ProcessManagerCompositionRoot.SetServiceProvider(serviceProvider);
    }

    private void RunHostedServices() // Quartz and database migration.
    {
        using IServiceScope scope = ProcessManagerCompositionRoot.CreateScope();
        IEnumerable<IHostedService> hostedServices = scope.ServiceProvider.GetServices<IHostedService>();
        
        Task.WhenAll(hostedServices.Select(s => s.StartAsync(CancellationToken.None)))
            .GetAwaiter().GetResult();
    }
}