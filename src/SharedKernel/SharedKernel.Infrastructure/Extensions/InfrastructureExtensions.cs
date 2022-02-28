using Serilog;
using System.Reflection;
using System.Collections.Generic;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Queries;
using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Logging;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.SharedKernel.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure
    (
        this IServiceCollection services,
        IList<Assembly> assemblies,
        string moduleName,
        ILogger logger
    )
    {
        services.AddCommands(assemblies);
        services.AddQueries(assemblies);
        services.AddEvents(assemblies);
        services.AddLogging(logger, moduleName);
        services.AddValidation(assemblies);
        services.AddSingleton<IClockService, ClockService>();
        services.AddHostedService<DatabaseInitializerHostedService>();
        services.AddModuleRequests();

        return services;
    }

    public static TOptions GetOptions<TOptions>(this IServiceCollection services, string sectionName) 
        where TOptions : new()
    {
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
        
        return configuration.GetOptions<TOptions>(sectionName);
    }

    public static TOptions GetOptions<TOptions>(this IConfiguration configuration, string sectionName)
        => configuration.GetSection(sectionName).Get<TOptions>();
    
    public static IServiceCollection AddValidation(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        services.AddFluentValidation(fv => fv
            .RegisterValidatorsFromAssemblies(assemblies, includeInternalTypes: true));

        return services;
    }
}