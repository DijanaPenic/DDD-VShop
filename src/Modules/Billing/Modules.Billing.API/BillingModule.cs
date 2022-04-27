using Serilog;
using System.Reflection;
using System.Collections.Generic;
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
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Billing.Infrastructure;
using VShop.Modules.Billing.Infrastructure.Configuration;
using VShop.Modules.Billing.Infrastructure.Configuration.Extensions;
using VShop.Modules.Billing.Infrastructure.DAL.Repositories;
using VShop.Modules.Billing.Infrastructure.DAL.Repositories.Contracts;

using Module = VShop.SharedKernel.Infrastructure.Modules.Module;

namespace VShop.Modules.Billing.API;

internal class BillingModule : Module
{
    public override IEnumerable<string> Policies { get; } = new[]
    {
        "payments"
    };

    public BillingModule(IEnumerable<Assembly> assemblies) 
        : base("Billing", assemblies) { }

    public override void Initialize
    (
        ILogger logger,
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        ConfigureContainer(logger, configuration, services);
        StartHostedServicesAsync(BillingCompositionRoot.ServiceProvider).GetAwaiter().GetResult();
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
        StripeOptions stripeOptions = configuration
            .GetOptions<StripeOptions>($"{Name}:{StripeOptions.Section}");
        
        services.AddStripe(stripeOptions);
        services.AddApplication(Assemblies);
        services.AddInfrastructure(Assemblies, Name, logger);
        services.AddPostgres(postgresOptions);
        services.AddEventStore(eventStoreOptions);
        services.AddTransient<IPaymentRepository, PaymentRepository>();
        services.AddSingleton(BillingMessageRegistry.Initialize());
        services.AddSingleton<IDispatcher, BillingDispatcher>();

        services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandlerDecorator<,>));
        services.TryDecorate(typeof(ICommandHandler<>), typeof(TransactionalCommandHandlerDecorator<>));
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(TransactionalCommandHandlerDecorator<,>));
        services.TryDecorate(typeof(IEventHandler<>), typeof(LoggingEventHandlerDecorator<>));
        
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        BillingCompositionRoot.SetServiceProvider(serviceProvider, FullName);
        
        ModuleRegistry.AddBroadcastActions(BillingCompositionRoot.ServiceProvider, Assemblies);
        ModuleEventStoreSubscriptionRegistry.Add(BillingCompositionRoot.ServiceProvider);
    }
}