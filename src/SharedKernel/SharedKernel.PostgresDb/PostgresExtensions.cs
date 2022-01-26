using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb.Contracts;

namespace VShop.SharedKernel.PostgresDb;

public static class PostgresExtensions
{    
    public static IServiceCollection AddUnitOfWork<T>(this IServiceCollection services) where T : class, IUnitOfWork
    {
        services.AddScoped<IUnitOfWork, T>();
        services.AddScoped<T>();

        return services;
    }
    
    public static IServiceCollection AddDbContextBuilder<T>(this IServiceCollection services, string connectionString)
        where T : DbContextBase
    {
        services.AddScoped<IDbContextBuilder>(_ => new DbContextBuilder
        (
            connectionString,
            typeof(T).Assembly
        ));

        return services;
    }
}