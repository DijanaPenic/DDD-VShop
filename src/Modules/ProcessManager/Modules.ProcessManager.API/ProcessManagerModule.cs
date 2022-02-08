using MediatR;
using Serilog;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.ProcessManager.Infrastructure;
using VShop.Modules.ProcessManager.Infrastructure.Configuration;
using VShop.Modules.ProcessManager.Infrastructure.Configuration.Extensions;
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Subscriptions;

using Module = VShop.SharedKernel.Infrastructure.Modules.Module;

namespace VShop.Modules.ProcessManager.API;

internal class ProcessManagerModule : Module
{
    public ProcessManagerModule(IEnumerable<Assembly> assemblies) : base("ProcessManager", assemblies) { }

    public override void Initialize
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor,
        IMessageContextRegistry messageContextRegistry
    )
    {
        ConfigureContainer(configuration, logger, contextAccessor, messageContextRegistry);
        StartHostedServices(ProcessManagerCompositionRoot.ServiceProvider);
    }

    public override void ConfigureContainer
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor,
        IMessageContextRegistry messageContextRegistry
    )
    {
        PostgresOptions postgresOptions = configuration
            .GetOptions<PostgresOptions>($"{Name}:Postgres");
        EventStoreOptions eventStoreOptions = configuration
            .GetOptions<EventStoreOptions>("EventStore");
        
        ServiceCollection services = new();
        
        services.AddInfrastructure(Assemblies, Name, logger, contextAccessor, messageContextRegistry);
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
        ProcessManagerCompositionRoot.SetServiceProvider(serviceProvider, FullName);
        
        ModuleRegistry.AddBroadcastActions(ProcessManagerCompositionRoot.ServiceProvider, Assemblies);
        ModuleEventStoreSubscriptionRegistry.Add(ProcessManagerCompositionRoot.ServiceProvider);
    }
}