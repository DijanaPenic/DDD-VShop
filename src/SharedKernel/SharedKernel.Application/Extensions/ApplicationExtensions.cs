using Serilog;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using NodaTime.Serialization.JsonNet;

using VShop.SharedKernel.Application.Providers;

namespace VShop.SharedKernel.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication
    (
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        List<string> disabledModules = new();
        foreach ((string key, string value) in configuration.AsEnumerable())
        {
            if (!key.Contains(":Module:Enabled")) continue;
            if (!bool.Parse(value)) disabledModules.Add(key.Split(":")[0]);
        }

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
        }).ConfigureApplicationPartManager(manager =>
        {
            List<ApplicationPart> removedParts = new();
            foreach (string disabledModule in disabledModules)
            {
                IEnumerable<ApplicationPart> parts = manager.ApplicationParts.Where(ap => 
                    ap.Name.Contains(disabledModule, StringComparison.InvariantCultureIgnoreCase));
                removedParts.AddRange(parts);
            }

            foreach (ApplicationPart part in removedParts)
            {
                manager.ApplicationParts.Remove(part);
            }
                
            manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
        });
        
        services.AddSwaggerGen(swagger =>
        {
            swagger.CustomSchemaIds(t => t.FullName);
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Modular API",
                Version = "v1"
            });
        });

        return services;
    }
    
    public static IApplicationBuilder UseApplication(this IApplicationBuilder app)
    {
        app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
        
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        
        app.UseSwagger();
        
        app.UseReDoc(reDoc =>
        {
            reDoc.RoutePrefix = "docs";
            reDoc.SpecUrl("/swagger/v1/swagger.json");
            reDoc.DocumentTitle = "Modular API";
        });
        
        app.UseRouting();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", context => context.Response.WriteAsync("Modular API"));
        });
        
        app.UseSerilogRequestLogging();

        return app;
    }

}