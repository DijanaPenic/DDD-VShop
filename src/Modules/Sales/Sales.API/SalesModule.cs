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
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.Modules.Sales.API.Automapper;
using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.Infrastructure.Queries;
using VShop.Modules.Sales.Infrastructure.Services;
using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.Modules.Sales.Infrastructure.Configuration.Extensions;

using ILogger = Serilog.ILogger;

namespace VShop.Modules.Sales.API;

internal class SalesModule : IModule
{
    public string Name => "Sales";
    public Assembly[] Assemblies { get; set; }

    public void Use(IConfiguration configuration, ILogger logger)
    {
        ConfigureCompositionRoot(configuration, logger);
        
        using IServiceScope scope = SalesCompositionRoot.CreateScope();
        IEnumerable<IHostedService> hostedServices = scope.ServiceProvider.GetServices<IHostedService>();

        Task.WhenAll(hostedServices.Select(s => s.StartAsync(CancellationToken.None)));
    }

    private void ConfigureCompositionRoot(IConfiguration configuration, ILogger logger)
    {
        ServiceCollection services = new();
        
        PostgresOptions postgresOptions = configuration.GetOptions<PostgresOptions>($"{Name}:Postgres");
        EventStoreOptions eventStoreOptions = configuration.GetOptions<EventStoreOptions>("EventStore");

        services.AddInfrastructure(Assemblies);
        services.AddPostgres(postgresOptions.ConnectionString);
        services.AddScheduler(postgresOptions.ConnectionString);
        services.AddEventStore(eventStoreOptions.ConnectionString);
        services.AddScoped<ISalesDispatcher, SalesDispatcher>();
        services.AddTransient<IShoppingCartReadService, ShoppingCartReadService>();
        services.AddTransient<IShoppingCartOrderingService, ShoppingCartOrderingService>();
        services.AddAutoMapper(typeof(ShoppingCartAutomapperProfile));
        services.AddSingleton(logger.ForContext("Module", "Sales"));
        services.AddSingleton<IMessageRegistry>(SalesMessageRegistry.Initialize());
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingCommandDecorator<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RetryPolicyCommandDecorator<,>));

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        SalesCompositionRoot.SetServiceProvider(serviceProvider);
    }
}