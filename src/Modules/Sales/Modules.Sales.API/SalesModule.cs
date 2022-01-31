using System;
using Serilog;
using MediatR;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Services.Contracts;
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;
using VShop.Modules.Sales.API.Automapper;
using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.Infrastructure.Queries;
using VShop.Modules.Sales.Infrastructure.Queries.Contracts;
using VShop.Modules.Sales.Infrastructure.Services;
using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.Modules.Sales.Infrastructure.Configuration.Extensions;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Modules;

namespace VShop.Modules.Sales.API;

internal class SalesModule : IModule
{
    public string Name => "Sales";
    public Assembly[] Assemblies { get; set; }

    public void Initialize
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor
    )
    {
        ConfigureCompositionRoot(configuration, logger, contextAccessor);

        ModuleRegistry.AddBroadcastActions(SalesCompositionRoot.ServiceProvider, Assemblies);
        
        IEnumerable<IEventStoreBackgroundService> subscriptionServices = SalesCompositionRoot.ServiceProvider
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
        // services.Decorate
        // (
        //     typeof(INotificationHandler<>),
        //     typeof(LoggingEventDecorator<>)
        // );

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        SalesCompositionRoot.SetServiceProvider(serviceProvider);
    }

    private void RunHostedServices() // Database migration.
    {
        using IServiceScope scope = SalesCompositionRoot.CreateScope();
        IEnumerable<IHostedService> hostedServices = scope.ServiceProvider.GetServices<IHostedService>();
        
        Task.WhenAll(hostedServices.Select(s => s.StartAsync(CancellationToken.None)))
            .GetAwaiter().GetResult();
    }
}