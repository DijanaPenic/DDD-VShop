using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb.Contracts;

namespace VShop.SharedKernel.PostgresDb;

// TODO - move to Extensions directory.
public static class PostgresExtensions
{    
    public static IServiceCollection AddUnitOfWork<T>(this IServiceCollection services) where T : class, IUnitOfWork
    {
        services.AddScoped<IUnitOfWork, T>();
        services.AddScoped<T>();

        return services;
    }

    public static IServiceCollection AddDbContextBuilder
    (
        this IServiceCollection services,
        string connectionString,
        Assembly assemblyType
    )
    {
        services.AddScoped<IDbContextBuilder>(_ => new DbContextBuilder(connectionString, assemblyType));

        return services;
    }

    public static IServiceCollection AddDbContextBuilder<T>(this IServiceCollection services, string connectionString)
        where T : DbContextBase
        => services.AddDbContextBuilder(connectionString, typeof(T).Assembly);
}