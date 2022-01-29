using Serilog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

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
}