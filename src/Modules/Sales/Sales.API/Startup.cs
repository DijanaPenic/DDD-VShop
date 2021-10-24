using Serilog;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using VShop.Modules.Sales.API.Application.Queries;
using VShop.Modules.Sales.API.Infrastructure.Extensions;
using VShop.Modules.Sales.API.Infrastructure.Automapper;
using VShop.Modules.Sales.API.Infrastructure.AutofacModules;

namespace VShop.Modules.Sales.API
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
            services.AddControllersServices();
            services.AddFluentValidationServices();
            services.AddAutoMapper(typeof(ShoppingCartAutomapperProfile));
            services.AddSwaggerGen(options => { options.SwaggerDoc("v1", new OpenApiInfo { Title = "Sales.API", Version = "v1" }); });
            services.AddEventStoreServices(Configuration.GetConnectionString("EventStoreDb"));
            services.AddPostgresServices(Configuration.GetConnectionString("PostgresDb"));
            
            services.AddTransient<IShoppingCartQueryService, ShoppingCartQueryService>();
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sales.API v1"));
            }

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}