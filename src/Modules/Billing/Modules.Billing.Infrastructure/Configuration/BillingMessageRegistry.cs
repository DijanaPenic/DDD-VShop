using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Billing.Integration.Events;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.Modules.Billing.Infrastructure.Configuration
{
    internal static class BillingMessageRegistry
    {
        public static IMessageRegistry Initialize()
        {
            MessageRegistry registry = new();

            registry.RegisterMessages();
            registry.RegisterTransformations();

            return registry;
        }
        
        private static IMessageRegistry RegisterMessages(this MessageRegistry registry)
        {
            // Configure integration events - local
            registry.Add<PaymentFailedIntegrationEvent>(nameof(PaymentFailedIntegrationEvent));
            registry.Add<PaymentSucceededIntegrationEvent>(nameof(PaymentSucceededIntegrationEvent));
            
            // Configure integration events - remote
            registry.Add<OrderFinalizedIntegrationEvent>(nameof(OrderFinalizedIntegrationEvent));

            return registry;
        }
        
        private static IMessageRegistry RegisterTransformations(this MessageRegistry registry)
        {
            return registry;
        }
    }
}