using MediatR;
using Serilog;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.Modules.Billing.API.Automapper;
using VShop.Modules.Billing.Infrastructure;
using VShop.Modules.Billing.Infrastructure.Services;
using VShop.Modules.Billing.Infrastructure.Services.Contracts;
using VShop.Modules.Billing.Infrastructure.Configuration;
using VShop.Modules.Billing.Infrastructure.Configuration.Extensions;
using VShop.Modules.Billing.Infrastructure.DAL.Repositories;
using VShop.Modules.Billing.Infrastructure.DAL.Repositories.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Subscriptions;

using Module = VShop.SharedKernel.Infrastructure.Modules.Module;

namespace VShop.Modules.Billing.API;

internal class BillingModule : Module
{
    public BillingModule(IEnumerable<Assembly> assemblies) : base("Billing", assemblies) { }

    public override void Initialize
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor,
        IMessageContextRegistry messageContextRegistry
    )
    {
        ConfigureContainer(configuration, logger, contextAccessor, messageContextRegistry);
        StartHostedServicesAsync(BillingCompositionRoot.ServiceProvider).GetAwaiter().GetResult();
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
        services.AddTransient<IPaymentService, FakePaymentService>();
        services.AddTransient<IPaymentRepository, PaymentRepository>();
        services.AddAutoMapper(typeof(PaymentAutomapperProfile));
        services.AddSingleton(BillingMessageRegistry.Initialize());
        services.AddSingleton<IBillingDispatcher, BillingDispatcher>();
        services.AddSingleton<IDispatcher, BillingDispatcher>();

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
        services.AddTransient
        (
            typeof(IPipelineBehavior<,>),
            typeof(TransactionalCommandDecorator<,>)
        );
        services.Decorate
        (
            typeof(INotificationHandler<>),
            typeof(LoggingEventDecorator<>)
        );

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        BillingCompositionRoot.SetServiceProvider(serviceProvider, FullName);
        
        ModuleRegistry.AddBroadcastActions(BillingCompositionRoot.ServiceProvider, Assemblies);
        ModuleEventStoreSubscriptionRegistry.Add(BillingCompositionRoot.ServiceProvider);
    }
}