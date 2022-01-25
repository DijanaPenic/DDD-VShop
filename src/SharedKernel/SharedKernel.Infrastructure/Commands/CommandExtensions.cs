using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.SharedKernel.Infrastructure.Commands;

internal static class CommandExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
        
        return services;
    }
}