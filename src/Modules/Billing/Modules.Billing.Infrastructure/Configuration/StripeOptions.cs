namespace VShop.Modules.Billing.Infrastructure.Configuration;

internal class StripeOptions
{
    public const string Section = "Stripe";
    public string PublishableKey { get; set; }
    public string SecretKey { get; set; }
    public string WebhookSecret { get; set; }
}