using VShop.Modules.Billing.Integration.Events;

using static VShop.SharedKernel.Messaging.MessageTypeMapper;

namespace VShop.Modules.Billing.API.Application
{
    public static class MessageMappings
    {
        public static void MapMessageTypes()
        {
            // Configure integration events
            AddCustomMap<PaymentFailedIntegrationEvent>(nameof(PaymentFailedIntegrationEvent));
            AddCustomMap<PaymentSucceededIntegrationEvent>(nameof(PaymentSucceededIntegrationEvent));
        }
    }
}