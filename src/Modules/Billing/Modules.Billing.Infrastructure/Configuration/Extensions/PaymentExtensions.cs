using Stripe;
using Microsoft.Extensions.DependencyInjection;

namespace VShop.Modules.Billing.Infrastructure.Configuration.Extensions;

internal static class PaymentExtensions
{
    public static void AddStripe(this IServiceCollection services, StripeOptions stripeOptions)
    {
        services.AddSingleton(stripeOptions);

        // TODO - singleton client?
        StripeClient stripeClient = new(stripeOptions.SecretKey);
        services.AddSingleton<IStripeClient>(stripeClient);
    }
}