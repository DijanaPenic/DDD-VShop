using Serilog;
using Microsoft.AspNetCore.Mvc.Controllers;

using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Services;
using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Auth;
using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Modules;

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
        services.AddAuth(_modules);
        services.AddSingleton(_configuration);

        ILogger logger = ConfigureLogger();

        foreach (Module module in _modules)
            module.Initialize(logger, _configuration, services.Clone());

        services.AddApplication(_configuration);
        services.AddSingleton<IControllerFactory, CustomControllerFactory>();

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