using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using VShop.Services.Basket.Infrastructure;
using VShop.Services.Basket.API.Application.Queries;

namespace VShop.Services.Basket.API.Infrastructure.Extensions
{
    public static class PostgresExtensions
    {
        public static void AddPostgresServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<BasketContext>(options => options.UseNpgsql
            (
                connectionString,
                // Target project needs to match migrations assembly
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name)
            )
            .UseSnakeCaseNamingConvention());

            services.AddTransient<IBasketQueryService, BasketQueryService>();
        }
    }
}