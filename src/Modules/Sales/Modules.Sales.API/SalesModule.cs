using System;
using System.Collections.Generic;
using System.Reflection;
using Serilog;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.Modules.Sales.API.Automapper;
using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.Infrastructure.Queries;
using VShop.Modules.Sales.Infrastructure.Queries.Contracts;
using VShop.Modules.Sales.Infrastructure.Services;
using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.Modules.Sales.Infrastructure.Configuration.Extensions;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Subscriptions;

using Module = VShop.SharedKernel.Infrastructure.Modules.Module;

namespace VShop.Modules.Sales.API;

internal class SalesModule : Module
{
    public SalesModule(IEnumerable<Assembly> assemblies) : base("Sales", assemblies) { }

    public override void Initialize
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor,
        IMessageContextRegistry messageContextRegistry
    )
    {
        ConfigureContainer(configuration, logger, contextAccessor, messageContextRegistry);
        StartHostedServices(SalesCompositionRoot.ServiceProvider); // TODO - integration tests review.
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
        services.AddTransient<IShoppingCartReadService, ShoppingCartReadService>();
        services.AddTransient<IShoppingCartOrderingService, ShoppingCartOrderingService>();
        services.AddAutoMapper(typeof(ShoppingCartAutomapperProfile));
        services.AddSingleton(SalesMessageRegistry.Initialize());
        services.AddSingleton<ISalesDispatcher, SalesDispatcher>();
        services.AddSingleton<IDispatcher, SalesDispatcher>();

        services.AddTransient
        (
            typeof(IPipelineBehavior<,>),
            typeof(LoggingCommandDecorator<,>)
        );
        services.AddTransient
        (
            typeof(IPipelineBehavior<,>),
            typeof(RetryPolicyCommandDecorator<,>)
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