using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Queries.Contracts;

namespace VShop.SharedKernel.Infrastructure.Queries;

internal static class QueryExtensions
{
    public static IServiceCollection AddQueries(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>))
                .Where(t => !t.IsAssignableTo(typeof(IDecorator))))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        return services;
    }
}