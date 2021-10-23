using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Application.Providers;

namespace VShop.Services.ShoppingCarts.API.Infrastructure.Extensions
{
    public static class ControllersExtensions
    {
        public static void AddControllersServices(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.ValueProviderFactories.Add(new SnakeCaseQueryValueProviderFactory());
                options.ModelBinderProviders.Insert(0, new GuidEntityBinderProvider());
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
            });
        }
    }
}