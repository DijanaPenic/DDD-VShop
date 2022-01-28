using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace VShop.SharedKernel.Infrastructure.Contexts;

internal static class ContextExtensions
{
    private const string CorrelationIdKey = "x-correlation-id";
    private const string ForwardedForKey = "x-forwarded-for";
    private const string RequestIdKey = "x-request-for";
    private const string UserAgentKey = "user-agent";

    public static string GetUserIpAddress(this HttpContext context)
    {
        if (context is null) return string.Empty;

        string ipAddress = context.Connection.RemoteIpAddress?.ToString();
        
        if (!context.Request.Headers.TryGetValue(ForwardedForKey, out StringValues forwardedFor))
            return ipAddress ?? string.Empty;
        
        string[] ipAddresses = forwardedFor.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries);
        if (ipAddresses.Any()) ipAddress = ipAddresses[0];
        
        return ipAddress ?? string.Empty;
    }

    public static Guid? TryGetCorrelationId(this HttpContext context)
        => context.Request.Headers.TryGetValue(CorrelationIdKey, out StringValues id) 
            ? Guid.TryParse(id.ToString(), out Guid guid) ? guid : null
            : null;
    
    public static Guid? TryGetRequestId(this HttpContext context)
        => context.Request.Headers.TryGetValue(RequestIdKey, out StringValues id) 
            ? Guid.TryParse(id.ToString(), out Guid guid) ? guid : null
            : null;

    public static string GetUserAgent(this HttpContext context)
        => context.Request.Headers[UserAgentKey];
    
    public static IServiceCollection AddContext(this IServiceCollection services)
    {
        services.AddSingleton<ContextAccessor>();
        services.AddTransient(_ => ContextAccessor.RequestContext);
            
        return services;
    }

    public static IApplicationBuilder UseContext(this IApplicationBuilder app)
    {
        app.Use((ctx, next) =>
        {
            ContextAccessor.RequestContext = new RequestContext(ctx);
                
            return next();
        });
            
        return app;
    }
}