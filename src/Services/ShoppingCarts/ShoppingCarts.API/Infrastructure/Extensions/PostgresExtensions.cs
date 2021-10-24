using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using VShop.Services.ShoppingCarts.Infrastructure;

namespace VShop.Services.ShoppingCarts.API.Infrastructure.Extensions
{
    public static class PostgresExtensions
    {
        public static void AddPostgresServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ShoppingCartContext>(options => options.UseNpgsql
            (
                connectionString,
                // Target project needs to match migrations assembly
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name)
            )
            .UseSnakeCaseNamingConvention());
        }
    }
}