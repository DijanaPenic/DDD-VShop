using MediatR;
using Serilog;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;
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

namespace VShop.Modules.Billing.API;

internal class BillingModule : IModule
{
    public string Name => "Billing";
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
        BillingCompositionRoot.SetServiceProvider(serviceProvider);
        
        ModuleRegistry.AddBroadcastActions(BillingCompositionRoot.ServiceProvider, Assemblies);
        ModuleEventStoreSubscriptionRegistry.Add(BillingCompositionRoot.ServiceProvider);
    }

    private void RunHostedServices() // Database migration.
    {
        using IServiceScope scope = BillingCompositionRoot.CreateScope();
        IEnumerable<IHostedService> hostedServices = scope.ServiceProvider.GetServices<IHostedService>();

        Task.WhenAll(hostedServices.Select(s => s.StartAsync(CancellationToken.None)))
            .GetAwaiter().GetResult();
    }
}