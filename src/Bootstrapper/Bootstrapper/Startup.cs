using Serilog;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Controllers;

using VShop.Bootstrapper.Authorization;
using VShop.Modules.Billing.Infrastructure;
using VShop.Modules.Catalog.Infrastructure;
using VShop.Modules.Identity.Infrastructure;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Services;
using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Auth;
using VShop.SharedKernel.Infrastructure.Auth.Constants;
using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.Modules.ProcessManager.Infrastructure;
using VShop.Modules.Sales.Infrastructure;

using ILogger = Serilog.ILogger;

namespace VShop.Bootstrapper;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly Module[] _modules;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
        _modules = ModuleLoader.LoadModules(configuration).ToArray();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddContext();
        services.AddMessaging();
        services.AddSingleton(_configuration);
        services.AddSingleton<ISalesDispatcher, SalesDispatcher>();
        services.AddSingleton<IBillingDispatcher, BillingDispatcher>();
        services.AddSingleton<ICatalogDispatcher, CatalogDispatcher>();
        services.AddSingleton<IIdentityDispatcher, IdentityDispatcher>();
        services.AddSingleton<IProcessManagerDispatcher, ProcessManagerDispatcher>();

        services.AddAuth(_modules);
        services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, ClientAuthenticationHandler>
        (
            ApplicationAuthSchemes.ClientAuthenticationScheme,
            _ => { }
        );

        ILogger logger = ConfigureLogger();

        foreach (Module module in _modules)
            module.Initialize(logger, _configuration, services.Clone());

        services.AddApplication(_configuration);
        services.AddValidation(_modules.SelectMany(m => m.Assemblies).ToArray());
        services.AddSingleton<IControllerActivator, ServiceBasedControllerActivator>();
        services.AddSingleton(ModuleEventStoreSubscriptionRegistry.Services);
        services.AddHostedService<EventStoreHostedService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
    {
        logger.Information
        (
            "Enabled modules: {Modules}",
            string.Join(", ", _modules.Select(m => m.Name))
        );

        app.UseAuth();
        app.UseContext();
        app.UseApplication();
        
        Array.Clear(_modules);
    }
    
    private ILogger ConfigureLogger() => new LoggerConfiguration()
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(_configuration)
        .CreateLogger();
}