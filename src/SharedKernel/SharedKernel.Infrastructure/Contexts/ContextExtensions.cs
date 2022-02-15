using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

namespace VShop.SharedKernel.Infrastructure.Contexts;

public static class ContextExtensions
{
    private const string CorrelationIdKey = "x-correlation-id";
    private const string ForwardedForKey = "x-forwarded-for";
    private const string RequestIdKey = "x-request-id";
    private const string UserAgentKey = "user-agent";

    internal static string GetUserIpAddress(this HttpContext context)
    {
        if (context is null) return string.Empty;

        string ipAddress = context.Connection.RemoteIpAddress?.ToString();
        
        if (!context.Request.Headers.TryGetValue(ForwardedForKey, out StringValues forwardedFor))
            return ipAddress ?? string.Empty;
        
        string[] ipAddresses = forwardedFor.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries);
        if (ipAddresses.Any()) ipAddress = ipAddresses[0];
        
        return ipAddress ?? string.Empty;
    }

    internal static Guid? TryGetCorrelationId(this HttpContext context)
        => context.Request.Headers.TryGetValue(CorrelationIdKey, out StringValues id) 
            ? Guid.TryParse(id.ToString(), out Guid guid) ? guid : null
            : null;
    
    internal static Guid? TryGetRequestId(this HttpContext context)
        => context.Request.Headers.TryGetValue(RequestIdKey, out StringValues id) 
            ? Guid.TryParse(id.ToString(), out Guid guid) ? guid : null
            : null;

    internal static string GetUserAgent(this HttpContext context)
        => context.Request.Headers[UserAgentKey];

    public static IServiceCollection AddContext
    (
        this IServiceCollection services,
        IContextAccessor contextAccessor = null
    )
    {
        contextAccessor ??= new ContextAccessor();
        services.AddSingleton(contextAccessor);
        services.AddTransient(_ => contextAccessor.Context);
        
        services.AddSingleton<IContextAdapter, ContextAdapter>();

        IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
        services.AddSingleton(httpContextAccessor);
        
        return services;
    }

    public static IApplicationBuilder UseContext(this IApplicationBuilder app)
    {
        app.Use((ctx, next) =>
        {
            ctx.RequestServices.GetRequiredService<IContextAccessor>().Context = new Context(ctx);
                
            return next();
        });
            
        return app;
    }
}