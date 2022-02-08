using MediatR;
using Serilog;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Catalog.API.Automapper;
using VShop.Modules.Catalog.Infrastructure;
using VShop.Modules.Catalog.Infrastructure.Configuration;
using VShop.Modules.Catalog.Infrastructure.Configuration.Extensions;
using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Subscriptions;

using Module = VShop.SharedKernel.Infrastructure.Modules.Module;

namespace VShop.Modules.Catalog.API;

internal class CatalogModule : Module
{
    public CatalogModule(IEnumerable<Assembly> assemblies) : base("Catalog", assemblies) { }

    public override void Initialize
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor,
        IMessageContextRegistry messageContextRegistry
    )
    {
        ConfigureContainer(configuration, logger, contextAccessor, messageContextRegistry);
        StartHostedServicesAsync(CatalogCompositionRoot.ServiceProvider).GetAwaiter().GetResult();
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
        services.AddEventStore(eventStoreOptions.ConnectionString);
        services.AddAutoMapper(typeof(CatalogAutomapperProfile));
        services.AddSingleton(CatalogMessageRegistry.Initialize());
        services.AddSingleton<ICatalogDispatcher, CatalogDispatcher>();
        services.AddSingleton<IDispatcher, CatalogDispatcher>();

        services.Decorate
        (
            typeof(INotificationHandler<>),
            typeof(TransactionalEventDecorator<>)
        );

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        CatalogCompositionRoot.SetServiceProvider(serviceProvider, FullName);
        
        ModuleRegistry.AddBroadcastActions(CatalogCompositionRoot.ServiceProvider, Assemblies);
        ModuleEventStoreSubscriptionRegistry.Add(CatalogCompositionRoot.ServiceProvider);
    }
}