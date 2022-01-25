using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Queries.Contracts;

namespace VShop.SharedKernel.Infrastructure.Queries;

internal static class QueryExtensions
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        services.AddSingleton<IQueryDispatcher, QueryDispatcher>();
        
        return services;
    }
}