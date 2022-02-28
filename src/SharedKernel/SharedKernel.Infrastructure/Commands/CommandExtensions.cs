using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.SharedKernel.Infrastructure.Commands;

internal static class CommandExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>))
                .Where(t => !t.IsAssignableTo(typeof(IDecorator))))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>))
                .Where(t => !t.IsAssignableTo(typeof(IDecorator))))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}