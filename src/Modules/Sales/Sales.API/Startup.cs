using Serilog;
using Autofac;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.API.Application.Queries;
using VShop.Modules.Sales.API.Infrastructure.Extensions;
using VShop.Modules.Sales.API.Infrastructure.Automapper;
using VShop.Modules.Sales.API.Infrastructure.AutofacModules;
using VShop.Modules.Sales.Infrastructure.Services;
using VShop.SharedKernel.Scheduler.Services;

namespace VShop.Modules.Sales.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersServices();
            services.AddFluentValidationServices();
            services.AddAutoMapper(typeof(ShoppingCartAutomapperProfile));
            services.AddSwaggerGen(options => { options.SwaggerDoc("v1", new OpenApiInfo { Title = "Sales.API", Version = "v1" }); });
            services.AddPostgresServices(Configuration.GetConnectionString("PostgresDb"));
            services.AddEventStoreServices(Configuration.GetConnectionString("EventStoreDb"));

            // Configure scheduler
            services.AddTransient<IMessagingService, MessagingService>();
            services.AddQuartzServices(Configuration.GetConnectionString("PostgresDb"));
            
            // Configure query services
            services.AddTransient<IShoppingCartQueryService, ShoppingCartQueryService>();
            
            // Configure domain services
            services.AddTransient<IShoppingCartOrderingService, ShoppingCartOrderingService>();
        }
        
        public static void ConfigureContainer(ContainerBuilder builder) 
            => builder.RegisterModule(new MediatorModule());

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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