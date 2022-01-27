using Serilog;
using Microsoft.AspNetCore.Mvc.Controllers;

using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Services;
using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;

using ILogger = Serilog.ILogger;

namespace Bootstrapper;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IList<IModule> _modules;
    private readonly ILogger _logger;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
        _modules = ModuleLoader.LoadModules(configuration);
        _logger = ConfigureLogger();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplication(_configuration);
        services.AddSingleton<IControllerFactory, CustomControllerFactory>();
        
        foreach (IModule module in _modules) module.Initialize(_configuration, _logger);
        
        services.AddSingleton(ModuleEventStoreSubscriptionRegistry.Services);
        services.AddHostedService<EventStoreHostedService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
    {
        logger.Information($"Enabled modules: {string.Join(", ", _modules.Select(m => m.Name))}");
        
        app.UseApplication();

        _modules.Clear();
    }
    
    private ILogger ConfigureLogger() => new LoggerConfiguration()
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(_configuration)
        .CreateLogger();
}