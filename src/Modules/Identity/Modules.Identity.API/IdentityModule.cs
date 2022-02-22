using MediatR;
using Serilog;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.Modules.Identity.Infrastructure;
using VShop.Modules.Identity.Infrastructure.Configuration;
using VShop.Modules.Identity.Infrastructure.Configuration.Extensions;

using Module = VShop.SharedKernel.Infrastructure.Modules.Module;

namespace VShop.Modules.Identity.API;

internal class IdentityModule : Module
{
    public override IEnumerable<string> Policies { get; } = new[]
    {
        "auth"
    };
    
    public IdentityModule(IEnumerable<Assembly> assemblies) 
        : base("Identity", assemblies) { }

    public override void Initialize
    (
        ILogger logger,
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        ConfigureContainer(logger, configuration, services);
        StartHostedServicesAsync(IdentityCompositionRoot.ServiceProvider).GetAwaiter().GetResult();
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

        services.AddIdentity();
        services.AddApplication(Assemblies);
        services.AddInfrastructure(Assemblies, Name, logger);
        services.AddPostgres(postgresOptions.ConnectionString);
        services.AddEventStore(eventStoreOptions.ConnectionString);
        services.AddSingleton(IdentityMessageRegistry.Initialize());
        services.AddSingleton<IDispatcher, IdentityDispatcher>();

        services.AddTransient
        (
            typeof(IPipelineBehavior<,>),
            typeof(LoggingCommandDecorator<,>)
        );
        services.AddTransient
        (
            typeof(IPipelineBehavior<,>),
            typeof(TransactionalCommandDecorator<,>)
        );

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        IdentityCompositionRoot.SetServiceProvider(serviceProvider, FullName);
        
        ModuleRegistry.AddBroadcastActions(IdentityCompositionRoot.ServiceProvider, Assemblies);
    }
}