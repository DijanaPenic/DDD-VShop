using Microsoft.AspNetCore.Builder;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.Infrastructure.Logging;

internal static class LoggingExtensions
{
    public static IServiceCollection AddLogging(this IServiceCollection services, ILogger logger, string module)
    {
        services.AddSingleton(logger.ForContext("Module", module));
        services.AddSingleton<ILoggerFactory>(new SerilogLoggerFactory());

        return services;
    }
        
    public static IApplicationBuilder UseLogging(this IApplicationBuilder app)
    {
        app.Use(async (ctx, next) =>
        {
            ILogger logger = ctx.RequestServices.GetRequiredService<ILogger>();
            IRequestContext requestContext = ctx.RequestServices.GetRequiredService<IRequestContext>();

            logger.Information
            (
                "Started processing a request [Request ID: '{RequestId}', Correlation ID: '{CorrelationId}', Trace ID: '{TraceId}', User ID: '{UserId}']...",
                requestContext.RequestId, requestContext.CorrelationId, requestContext.TraceId,
                requestContext.Identity.IsAuthenticated ? requestContext.Identity.Id : string.Empty
            );

            await next();

            logger.Information
            (
                "Finished processing a request with status code: {StatusCode} [Request ID: '{RequestId}', Correlation ID: '{CorrelationId}', Trace ID: '{TraceId}', User ID: '{UserId}']",
                ctx.Response.StatusCode, requestContext.RequestId, requestContext.CorrelationId, requestContext.TraceId,
                requestContext.Identity.IsAuthenticated ? requestContext.Identity.Id : string.Empty
            );
        });

        return app;
    }
}