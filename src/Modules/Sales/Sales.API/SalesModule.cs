using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Sales.API.Automapper;
using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.Modules.Sales.Infrastructure.Configuration.Extensions;
using VShop.Modules.Sales.Infrastructure.Queries;
using VShop.Modules.Sales.Infrastructure.Services;

using ILogger = Serilog.ILogger;

namespace VShop.Modules.Sales.API;

public class SalesModule : IModule
{
    public string Name => "Sales";
    public Assembly[] Assemblies { get; set; }
    public IServiceProvider ServiceProvider { get; private set; }

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

        services.AddPostgres(postgresOptions.ConnectionString);
        services.AddScheduler(postgresOptions.ConnectionString);
        services.AddEventStore(eventStoreOptions.ConnectionString);
        services.AddInfrastructure(Assemblies);
        services.AddScoped<ISalesDispatcher, SalesDispatcher>();
        services.AddTransient<IShoppingCartReadService, ShoppingCartReadService>();
        services.AddTransient<IShoppingCartOrderingService, ShoppingCartOrderingService>();
        services.AddAutoMapper(typeof(ShoppingCartAutomapperProfile));
        services.AddSingleton(_ => logger.ForContext("Module", "Sales"));

        services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingCommandDecorator<,>));
        services.TryDecorate(typeof(ICommandHandler<>), typeof(RetryPolicyCommandDecorator<,>));
        
        ServiceProvider = services.BuildServiceProvider();
        SalesCompositionRoot.SetServiceProvider(ServiceProvider);
    }
}