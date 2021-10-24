using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using VShop.Services.Sales.Infrastructure;

namespace VShop.Services.Sales.API.Infrastructure.Extensions
{
    public static class PostgresExtensions
    {
        public static void AddPostgresServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SalesContext>(options => options.UseNpgsql
            (
                connectionString,
                // Target project needs to match migrations assembly
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name)
            )
            .UseSnakeCaseNamingConvention());
        }
    }
}