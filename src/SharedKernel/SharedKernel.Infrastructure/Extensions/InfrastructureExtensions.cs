using System;
using MediatR;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Queries;
using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

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
        
        //services.AddModuleRequests(assemblies); // TODO - can be done later.
        //services.AddContext();  // TODO - complex; can be done later.
        //services.AddMessaging();  // TODO - need to finish.

        //app.UseCorrelationId();
        //app.UseContext();
        //app.UseLogging(); 

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