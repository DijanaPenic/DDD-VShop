using System;
using System.Reflection;
using System.Collections.Generic;
using Serilog;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.Modules.Sales.API.Automapper;
using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.Infrastructure.Queries;
using VShop.Modules.Sales.Infrastructure.Queries.Contracts;
using VShop.Modules.Sales.Infrastructure.Services;
using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.Modules.Sales.Infrastructure.Configuration.Extensions;

using Module = VShop.SharedKernel.Infrastructure.Modules.Module;

namespace VShop.Modules.Sales.API;

internal class SalesModule : Module
{
    public override IEnumerable<string> Policies { get; } = new[]
    {
        "orders", 
        "shopping_carts"
    };
    
    public SalesModule(IEnumerable<Assembly> assemblies) 
        : base("Sales", assemblies) { }

    public override void Initialize
    (
        ILogger logger,
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        ConfigureContainer(logger, configuration, services);
        StartHostedServicesAsync(SalesCompositionRoot.ServiceProvider).GetAwaiter().GetResult();
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
        services.AddPostgres(postgresOptions.ConnectionString);
        services.AddEventStore(eventStoreOptions.ConnectionString);
        services.AddTransient<IShoppingCartReadService, ShoppingCartReadService>();
        services.AddTransient<IShoppingCartOrderingService, ShoppingCartOrderingService>();
        services.AddAutoMapper(typeof(ShoppingCartAutomapperProfile));
        services.AddSingleton(SalesMessageRegistry.Initialize());
        services.AddSingleton<IDispatcher, SalesDispatcher>();

        services.AddTransient
        (
            typeof(IPipelineBehavior<,>),
            typeof(LoggingCommandDecorator<,>)
        );
        // services.Decorate // Note: right now, there are no event handlers in the Sales module.
        // (
        //     typeof(INotificationHandler<>),
        //     typeof(LoggingEventDecorator<>)
        // );

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        SalesCompositionRoot.SetServiceProvider(serviceProvider, FullName);
        
        ModuleRegistry.AddBroadcastActions(SalesCompositionRoot.ServiceProvider, Assemblies);
        ModuleEventStoreSubscriptionRegistry.Add(SalesCompositionRoot.ServiceProvider);
    }
}