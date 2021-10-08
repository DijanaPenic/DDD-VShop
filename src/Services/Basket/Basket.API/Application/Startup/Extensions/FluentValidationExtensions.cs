using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace VShop.Services.Basket.API.Application.Startup.Extensions
{
    public static class FluentValidationExtensions
    {
        public static void AddFluentValidationServices(this IServiceCollection services)
        {
            services.AddFluentValidation(config =>
            {
                config.RegisterValidatorsFromAssembly(typeof(API.Startup).Assembly);
                config.ValidatorOptions.CascadeMode = CascadeMode.Stop;
            });
        }
    }
}