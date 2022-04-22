using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb.Contracts;

namespace VShop.SharedKernel.PostgresDb.Extensions;

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
        Assembly migrationAssembly
    )
    {
        services.AddScoped<IDbContextBuilder>(_ => new DbContextBuilder(connectionString, migrationAssembly));
        return services;
    }
}