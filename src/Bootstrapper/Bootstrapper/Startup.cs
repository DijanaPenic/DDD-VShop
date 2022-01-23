using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;
using VShop.SharedKernel.Infrastructure.Services;

using ILogger = Serilog.ILogger;

namespace Bootstrapper;

public class Startup
{
    private const string ModulePrefix = "VShop.Modules.";
    
    private readonly IConfiguration _configuration;
    private readonly IList<Assembly> _assemblies;
    private readonly IList<IModule> _modules;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
        
        _assemblies = ModuleLoader.LoadAssemblies(configuration, ModulePrefix);
        _modules = ModuleLoader.LoadModules(_assemblies, ModulePrefix);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplication(_configuration);
        services.AddSingleton<IControllerFactory, CustomControllerFactory>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
    {
        logger.Information($"Enabled modules: {string.Join(", ", _modules.Select(m => m.Name))}");
        
        app.UseApplication();
        
        foreach (IModule module in _modules) module.Use(_configuration, logger);
        
        _assemblies.Clear();
        _modules.Clear();
    }
}