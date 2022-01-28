using System.Reflection;
using MediatR;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Queries;
using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Logging;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.SharedKernel.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure
    (
        this IServiceCollection services,
        Assembly[] assemblies,
        string module,
        ILogger logger
    )
    {
        services.AddMediatR(assemblies);
        services.AddCommands();
        services.AddQueries();
        services.AddEvents();
        services.AddMemoryCache();
        services.AddContext();
        services.AddSingleton<IClockService, ClockService>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddHostedService<DatabaseInitializerHostedService>();
        services.AddFluentValidation(assemblies);
        services.AddLogging(logger, module);
        services.AddMessaging(); 

        // TODO - need to finish.
        //services.AddModuleRequests(assemblies); 

        return services;
    }
    
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseContext();
        app.UseLogging(); 

        return app;
    }

    public static TOptions GetOptions<TOptions>(this IConfiguration configuration, string sectionName)
        => configuration.GetSection(sectionName).Get<TOptions>();
    
    private static IServiceCollection AddFluentValidation(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddFluentValidation(config =>
        {
            config.RegisterValidatorsFromAssemblies(assemblies);
            config.ValidatorOptions.CascadeMode = CascadeMode.Stop;
        });

        return services;
    }
}