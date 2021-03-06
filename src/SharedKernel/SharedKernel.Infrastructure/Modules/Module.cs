using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VShop.SharedKernel.Infrastructure.Modules;

public abstract class Module
{
    public const string Prefix = "VShop.Modules.";
    public string Name { get; }
    protected string FullName => $"{Prefix}{Name}";
    public virtual IEnumerable<string> Policies
    {
        get { yield break; }
    }

    public virtual bool AutomaticValidationEnabled => false;

    public IList<Assembly> Assemblies { get; }

    protected Module(string name, IEnumerable<Assembly> assemblies)
    {
        Name = name;
        Assemblies = GetModuleAssemblies(assemblies);
    }

    public abstract void Initialize(ILogger logger, IConfiguration configuration, IServiceCollection services);
    public abstract void ConfigureContainer(ILogger logger, IConfiguration configuration, IServiceCollection services);
    
    public static Task StartHostedServicesAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IEnumerable<IHostedService> hostedServices = scope.ServiceProvider.GetServices<IHostedService>();
        
        return Task.WhenAll(hostedServices.Select(s => s.StartAsync(cancellationToken)));
    }
    
    public static Task StopHostedServicesAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IEnumerable<IHostedService> hostedServices = scope.ServiceProvider.GetServices<IHostedService>();
        
        return Task.WhenAll(hostedServices.Select(s => s.StopAsync(cancellationToken)));
    }
    
    private IList<Assembly> GetModuleAssemblies(IEnumerable<Assembly> assemblies) 
        => assemblies.Where(a => a.FullName is not null && a.FullName.StartsWith(FullName))
            .ToList();
}