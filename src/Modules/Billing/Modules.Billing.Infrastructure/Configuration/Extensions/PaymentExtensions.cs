using Stripe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.Modules.Billing.Infrastructure.Configuration.Extensions;

internal static class PaymentExtensions
{
    // TODO - cleanup.
    public static void AddStripe(this IServiceCollection services, IConfiguration configuration, string module)
    {
        StripeOptions stripeOptions = configuration
            .GetOptions<StripeOptions>($"{module}:{StripeOptions.Section}");
        
        services.AddSingleton(stripeOptions);

        // TODO - singleton client?
        StripeClient stripeClient = new(stripeOptions.SecretKey);
        services.AddSingleton<IStripeClient>(stripeClient);
    }
}