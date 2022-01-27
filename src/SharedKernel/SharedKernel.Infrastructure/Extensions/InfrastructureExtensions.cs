using System;
using System.Reflection;
using MediatR;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Extensions.Logging;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Queries;
using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddMediatR(assemblies);
        services.AddCommands();
        services.AddQueries();
        services.AddEvents();
        services.AddSingleton<IClockService, ClockService>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddHostedService<DatabaseInitializerHostedService>();
        services.AddMemoryCache();
        services.AddFluentValidation(assemblies);
        
        // TODO - need to finish.
        //services.AddModuleRequests(assemblies); 
        //services.AddContext();  
        //services.AddMessaging();  

        //app.UseCorrelationId();
        //app.UseContext();
        //app.UseLogging(); 

        return services;
    }
    
    public static IServiceCollection AddFluentValidation(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddFluentValidation(config =>
        {
            config.RegisterValidatorsFromAssemblies(assemblies);
            config.ValidatorOptions.CascadeMode = CascadeMode.Stop;
        });

        return services;
    }
    
    public static IServiceCollection AddLogging(this IServiceCollection services, ILogger logger, string module)
    {
        services.AddSingleton(logger.ForContext("Module", module));
        services.AddSingleton<ILoggerFactory>(new SerilogLoggerFactory());

        return services;
    }
    
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration, string sectionName)
        => configuration.GetSection(sectionName).Get<TOptions>();
    
    public static string GetModuleName(this object value) => value?.GetType().GetModuleName() ?? string.Empty;
    
    public static string GetModuleName(this Type type, string namespacePart = "Modules", int splitIndex = 2)
    {
        if (type?.Namespace is null) return string.Empty;

        return type.Namespace.Contains(namespacePart)
            ? type.Namespace.Split(".")[splitIndex].ToLowerInvariant()
            : string.Empty;
    }
}