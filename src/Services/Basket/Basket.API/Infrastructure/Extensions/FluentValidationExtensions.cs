using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace VShop.Services.Basket.API.Infrastructure.Extensions
{
    //TODO - potentially move to DI directory. Right now code is located in general Extensions directory.
    public static class FluentValidationExtensions
    {
        public static void AddFluentValidationServices(this IServiceCollection services)
        {
            services.AddFluentValidation(config =>
            {
                config.RegisterValidatorsFromAssembly(typeof(Startup).Assembly);
                config.ValidatorOptions.CascadeMode = CascadeMode.Stop;
            });
        }
    }
}