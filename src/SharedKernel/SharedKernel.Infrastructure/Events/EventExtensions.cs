using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Infrastructure.Events;

internal static class EventExtensions
{
    public static IServiceCollection AddEvents(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        services.AddSingleton<IEventDispatcher, EventDispatcher>();
        
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>))
                .Where(t => !t.IsAssignableTo(typeof(IDecorator))))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        return services;
    }
}