using Serilog;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using VShop.Services.ShoppingCarts.API.Infrastructure.Extensions;
using VShop.Services.ShoppingCarts.API.Infrastructure.Automapper;
using VShop.Services.ShoppingCarts.API.Infrastructure.AutofacModules;

namespace VShop.Services.ShoppingCarts.API
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
            services.AddAutoMapper(typeof(ShoppingCartAutomapperProfile));
            services.AddSwaggerGen(options => { options.SwaggerDoc("v1", new OpenApiInfo { Title = "ShoppingCarts.API", Version = "v1" }); });
            services.AddEventStoreServices(Configuration.GetConnectionString("EventStoreDb"));
            services.AddPostgresServices(Configuration.GetConnectionString("PostgresDb"));

            // TODO - missing DI for domain handlers
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShoppingCarts.API v1"));
            }

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}