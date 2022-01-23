using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using NodaTime.Serialization.JsonNet;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.API.Providers;

namespace VShop.Modules.Catalog.API.Infrastructure.Extensions
{
    public static class ControllersExtensions
    {
        public static void AddControllersServices(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.ValueProviderFactories.Add(new SnakeCaseQueryValueProviderFactory());
                options.ModelBinderProviders.Insert(0, new GuidModelBinderProvider());
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters = new List<JsonConverter>
                {
                    new StringEnumConverter(),
                    NodaConverters.InstantConverter
                };
                options.SerializerSettings.DateParseHandling = DateParseHandling.None;
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