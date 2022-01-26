using MediatR;
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
using VShop.Modules.Billing.Infrastructure.Configuration;
using VShop.Modules.Billing.Infrastructure.Configuration.Extensions;
using VShop.Modules.Billing.Infrastructure.DAL.Repositories;
using VShop.Modules.Billing.Infrastructure.DAL.Repositories.Contracts;
using VShop.Modules.Billing.Infrastructure.Services;
using VShop.Modules.Billing.Infrastructure.Services.Contracts;

using ILogger = Serilog.ILogger;

namespace VShop.Modules.Billing.API;

internal class BillingModule : IModule
{
    public string Name => "Billing";
    public Assembly[] Assemblies { get; set; }

    public void Use(IConfiguration configuration, ILogger logger)
    {
        ConfigureCompositionRoot(configuration, logger);
        
        using IServiceScope scope = BillingCompositionRoot.CreateScope();
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
        services.AddEventStore(eventStoreOptions.ConnectionString);
        services.AddScoped<IBillingDispatcher, BillingDispatcher>();
        services.AddTransient<IPaymentService, FakePaymentService>();
        services.AddTransient<IPaymentRepository, PaymentRepository>();
        services.AddAutoMapper(typeof(PaymentAutomapperProfile));
        services.AddSingleton(logger.ForContext("Module", "Billing"));
        services.AddSingleton(BillingMessageRegistry.Initialize());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingCommandDecorator<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RetryPolicyCommandDecorator<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionalCommandDecorator<,>));

        services.Decorate(typeof(INotificationHandler<>), typeof(LoggingEventDecorator<>));

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        BillingCompositionRoot.SetServiceProvider(serviceProvider);
    }
}