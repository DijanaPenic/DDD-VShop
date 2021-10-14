using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventStore.ClientAPI;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using VShop.SharedKernel.EventStore;
using VShop.Services.Basket.Infrastructure;
using VShop.Services.Basket.API.Infrastructure.Extensions;
using VShop.Services.Basket.API.Infrastructure.Automapper;
using VShop.Services.Basket.API.Infrastructure.AutofacModules;

namespace VShop.Services.Basket.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public ILifetimeScope AutofacContainer { get; private set; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddFluentValidationServices();
            services.AddAutoMapper(typeof(BasketAutomapperProfile));
            
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Basket.API", Version = "v1"}); });
            
            // Configure event store
            // TODO - move to extension class
            IEventStoreConnection esConnection = EventStoreConnection.Create
            (
                Configuration.GetConnectionString("EventStore"),
                ConnectionSettings.Create().KeepReconnecting().DisableTls(),
                "Basket.API" // TODO - change name.
            );
            services.AddSingleton(esConnection);
            services.AddSingleton(typeof(IEventStoreRepository<,>), typeof(EventStoreRepository<,>));
            services.AddSingleton<IHostedService, EventStoreService>();
            EventMappings.MapEventTypes();
            
            // TODO - missing DI for Postgres database and domain handlers
        }
        
        public static void ConfigureContainer(ContainerBuilder builder) 
        {
            builder.RegisterModule(new MediatorModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}