using Serilog;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using EventStore.Client;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using NodaTime.Serialization.JsonNet;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Extensions;
using VShop.SharedKernel.Application.Projections;
using VShop.SharedKernel.Application.Providers;
using VShop.SharedKernel.EventStoreDb.Messaging.Contracts;
using VShop.SharedKernel.EventStoreDb.Serialization.Contracts;

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
        })
        .ConfigureApplicationPartManager(manager =>
        {
            List<ApplicationPart> removedParts = new();
            foreach (string disabledModule in disabledModules)
            {
                IEnumerable<ApplicationPart> parts = manager.ApplicationParts.Where(ap => 
                    ap.Name.Contains(disabledModule, StringComparison.InvariantCultureIgnoreCase));
                removedParts.AddRange(parts);
            }

            foreach (ApplicationPart part in removedParts)
                manager.ApplicationParts.Remove(part);

            manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
        });
        
        // TODO - resolve Swagger.
        // services.AddSwaggerGen(swagger =>
        // {
        //     swagger.CustomSchemaIds(t => t.FullName);
        //     swagger.SwaggerDoc("v1", new OpenApiInfo
        //     {
        //         Title = "Modular API",
        //         Version = "v1"
        //     });
        // });

        return services;
    }

    public static IServiceCollection AddReadModelBackgroundService<TDbContext>
    (
        this IServiceCollection services,
        string subscriptionId,
        DomainEventProjectionToPostgres<TDbContext>.Projector projector,
        string aggregateStreamPrefix
    ) where TDbContext : DbContextBase
    {
        services.AddEventStoreBackgroundService(provider =>
        {
            ILogger logger = provider.GetService<ILogger>();
            IEventStoreMessageConverter eventStoreMessageConverter = provider.GetService<IEventStoreMessageConverter>();
            IEventStoreSerializer eventStoreSerializer = provider.GetService<IEventStoreSerializer>();

            return new SubscriptionConfig
            (
                subscriptionId,
                new DomainEventProjectionToPostgres<TDbContext>
                (
                    logger, provider, eventStoreMessageConverter, 
                    eventStoreSerializer, projector
                ),
                new SubscriptionFilterOptions(StreamFilter.Prefix(aggregateStreamPrefix))
            );
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
        
        // app.UseSwagger();
        //
        // app.UseReDoc(reDoc =>
        // {
        //     reDoc.RoutePrefix = "docs";
        //     reDoc.SpecUrl("/swagger/v1/swagger.json");
        //     reDoc.DocumentTitle = "Modular API";
        // });
        //
        app.UseRouting();

        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", context => context.Response.WriteAsync("Modular API"));
        });
        
        app.UseSerilogRequestLogging();

        return app;
    }
    
    public static IServiceCollection AddApplication(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddControllersAsServices(assemblies);

        return services;
    }

    private static IServiceCollection AddControllersAsServices(this IServiceCollection services, Assembly[] assemblies)
    {
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(f => f.AssignableTo(typeof(ControllerBase)))
            .AsSelf()
            .WithTransientLifetime());

        return services;
    }
}