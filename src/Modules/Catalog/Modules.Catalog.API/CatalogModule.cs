using MediatR;
using Serilog;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
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
using VShop.SharedKernel.Infrastructure.Modules.Contracts;
using VShop.SharedKernel.Subscriptions;

namespace VShop.Modules.Catalog.API;

internal class CatalogModule : IModule
{
    public string Name => "Catalog";
    public Assembly[] Assemblies { get; set; }

    public void Initialize
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor,
        IMessageContextRegistry messageContextRegistry
    )
    {
        ConfigureCompositionRoot(configuration, logger, contextAccessor, messageContextRegistry);
        RunHostedServices();
    }

    public void ConfigureCompositionRoot
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
        CatalogCompositionRoot.SetServiceProvider(serviceProvider);
        
        ModuleRegistry.AddBroadcastActions(CatalogCompositionRoot.ServiceProvider, Assemblies);
        ModuleEventStoreSubscriptionRegistry.Add(CatalogCompositionRoot.ServiceProvider);
    }

    private void RunHostedServices() // Database migration.
    {
        using IServiceScope scope = CatalogCompositionRoot.CreateScope();
        IEnumerable<IHostedService> hostedServices = scope.ServiceProvider.GetServices<IHostedService>();
        
        Task.WhenAll(hostedServices.Select(s => s.StartAsync(CancellationToken.None)))
            .GetAwaiter().GetResult();
    }
}