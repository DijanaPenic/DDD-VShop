using MediatR;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Catalog.API.Automapper;
using VShop.Modules.Catalog.Infrastructure.Configuration;
using VShop.Modules.Catalog.Infrastructure.Configuration.Extensions;
using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Services.Contracts;
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;

using ILogger = Serilog.ILogger;

namespace VShop.Modules.Catalog.API;

internal class CatalogModule : IModule
{
    public string Name => "Catalog";
    public Assembly[] Assemblies { get; set; }

    public void Initialize
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor
    )
    {
        ConfigureCompositionRoot(configuration, logger, contextAccessor);
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
        services.AddAutoMapper(typeof(CatalogAutomapperProfile));
        services.AddSingleton(CatalogMessageRegistry.Initialize());

        services.Decorate
        (
            typeof(INotificationHandler<>),
            typeof(TransactionalEventDecorator<>)
        );

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        CatalogCompositionRoot.SetServiceProvider(serviceProvider);

        ModuleRegistry.AddBroadcastActions(serviceProvider, Assemblies);

        IEnumerable<IEventStoreBackgroundService> subscriptionServices = serviceProvider
            .GetServices<IEventStoreBackgroundService>();
        ModuleEventStoreSubscriptionRegistry.Add(subscriptionServices);
    }

    private void RunHostedServices() // Database migration.
    {
        using IServiceScope scope = CatalogCompositionRoot.CreateScope();
        IEnumerable<IHostedService> hostedServices = scope.ServiceProvider.GetServices<IHostedService>();
        
        Task.WhenAll(hostedServices.Select(s => s.StartAsync(CancellationToken.None)))
            .GetAwaiter().GetResult();
    }
}