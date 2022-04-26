using Serilog;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.Modules.Catalog.API.Automapper;
using VShop.Modules.Catalog.Infrastructure;
using VShop.Modules.Catalog.Infrastructure.Configuration;
using VShop.Modules.Catalog.Infrastructure.Configuration.Extensions;

using Module = VShop.SharedKernel.Infrastructure.Modules.Module;

namespace VShop.Modules.Catalog.API;

internal class CatalogModule : Module
{
    public override IEnumerable<string> Policies { get; } = new[]
    {
        "categories", 
        "products"
    };

    public override bool AutomaticValidationEnabled => true;

    public CatalogModule(IEnumerable<Assembly> assemblies) 
        : base("Catalog", assemblies) { }
    
    public override void Initialize
    (
        ILogger logger,
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        ConfigureContainer(logger, configuration, services);
        StartHostedServicesAsync(CatalogCompositionRoot.ServiceProvider).GetAwaiter().GetResult();
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
        
        services.AddApplication(Assemblies);
        services.AddInfrastructure(Assemblies, Name, logger);
        services.AddPostgres(postgresOptions);
        services.AddEventStore(eventStoreOptions);
        services.AddAutoMapper(typeof(CatalogAutomapperProfile));
        services.AddSingleton(CatalogMessageRegistry.Initialize());
        services.AddSingleton<IDispatcher, CatalogDispatcher>();

        services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingEventHandlerDecorator<>));
        services.TryDecorate(typeof(IEventHandler<>), typeof(TransactionalEventHandlerDecorator<>));
        
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        CatalogCompositionRoot.SetServiceProvider(serviceProvider, FullName);
        
        ModuleRegistry.AddBroadcastActions(CatalogCompositionRoot.ServiceProvider, Assemblies);
        ModuleEventStoreSubscriptionRegistry.Add(CatalogCompositionRoot.ServiceProvider);
    }
}