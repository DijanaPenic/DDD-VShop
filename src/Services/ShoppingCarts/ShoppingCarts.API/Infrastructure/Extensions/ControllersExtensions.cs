using Newtonsoft.Json.Converters;
using Microsoft.Extensions.DependencyInjection;

namespace VShop.Services.ShoppingCarts.API.Infrastructure.Extensions
{
    public static class ControllersExtensions
    {
        public static void AddControllersServices(this IServiceCollection services)
        {
            services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
        }
    }
}