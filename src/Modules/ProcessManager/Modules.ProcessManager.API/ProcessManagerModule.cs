using Serilog;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.Modules.ProcessManager.Infrastructure;
using VShop.Modules.ProcessManager.Infrastructure.Configuration;
using VShop.Modules.ProcessManager.Infrastructure.Configuration.Extensions;

using Module = VShop.SharedKernel.Infrastructure.Modules.Module;

namespace VShop.Modules.ProcessManager.API;

internal class ProcessManagerModule : Module
{
    public ProcessManagerModule(IEnumerable<Assembly> assemblies) 
        : base("ProcessManager", assemblies) { }

    public override void Initialize
    (
        ILogger logger,
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        ConfigureContainer(logger, configuration, services);
        StartHostedServicesAsync(ProcessManagerCompositionRoot.ServiceProvider).GetAwaiter().GetResult();
    }

    public override void ConfigureContainer
    (
        ILogger logger,
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        PostgresOptions postgresOptions = configuration
            .GetOptions<PostgresOptions>($"{Name}:Postgres");
        EventStoreOptions eventStoreOptions = configuration
            .GetOptions<EventStoreOptions>(EventStoreOptions.Section);
        
        services.AddInfrastructure(Assemblies, Name, logger);
        services.AddPostgres(postgresOptions);
        services.AddScheduler(postgresOptions);
        services.AddEventStore(eventStoreOptions);
        services.AddSingleton(ProcessManagerMessageRegistry.Initialize());
        services.AddSingleton<IDispatcher, ProcessManagerDispatcher>();
        
        services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingEventHandlerDecorator<>));

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        ProcessManagerCompositionRoot.SetServiceProvider(serviceProvider, FullName);
        
        ModuleRegistry.AddBroadcastActions(ProcessManagerCompositionRoot.ServiceProvider, Assemblies);
        ModuleEventStoreSubscriptionRegistry.Add(ProcessManagerCompositionRoot.ServiceProvider);
    }
}