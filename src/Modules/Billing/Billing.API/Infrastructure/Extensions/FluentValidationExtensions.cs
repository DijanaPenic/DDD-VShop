using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace VShop.Modules.Billing.API.Infrastructure.Extensions
{
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