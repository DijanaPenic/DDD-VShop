using Autofac;
using Serilog;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Billing.API.Application;
using VShop.Modules.Billing.API.Infrastructure.Automapper;
using VShop.Modules.Billing.API.Infrastructure.Extensions;
using VShop.Modules.Billing.API.Infrastructure.AutofacModules;
using VShop.Modules.Billing.Infrastructure.Services;

namespace VShop.Modules.Billing.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersServices();
            services.AddFluentValidationServices();
            services.AddAutoMapper(typeof(PaymentAutomapperProfile));
            services.AddSwaggerGen(options => { options.SwaggerDoc("v1", new OpenApiInfo { Title = "Billing.API", Version = "v1" }); });
            services.AddPostgresServices(Configuration.GetConnectionString("PostgresDb"));
            services.AddIntegrationServices(Configuration.GetConnectionString("EventStoreDb"));

            services.AddTransient<IPaymentService, FakePaymentService>();
            MessageMappings.MapMessageTypes();
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Billing.API v1"));
            }

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}