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

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Modules;

public abstract class Module
{
    public const string Prefix = "VShop.Modules.";
    public string Name { get; }
    protected string FullName => $"{Prefix}{Name}";
    protected Assembly[] Assemblies { get; }

    protected Module(string name, IEnumerable<Assembly> assemblies)
    {
        Name = name;
        Assemblies = GetModuleAssemblies(assemblies);
    }

    public abstract void Initialize
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor,
        IMessageContextRegistry messageContextRegistry
    );

    public abstract void ConfigureContainer
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor,
        IMessageContextRegistry messageContextRegistry
    );
    
    protected static void StartHostedServices(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IEnumerable<IHostedService> hostedServices = scope.ServiceProvider.GetServices<IHostedService>();
        
        Task.WhenAll(hostedServices.Select(s => s.StartAsync(CancellationToken.None)))
            .GetAwaiter().GetResult();
    }
    
    private Assembly[] GetModuleAssemblies(IEnumerable<Assembly> assemblies) => assemblies
        .Where(a => a.FullName is not null && a.FullName.StartsWith(FullName))
        .ToArray();
}