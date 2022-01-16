using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Billing.Integration.Events;

using static VShop.SharedKernel.Messaging.MessageTypeMapper;
using static VShop.SharedKernel.Messaging.MessageTransformations;

namespace VShop.Modules.Billing.API.Application
{
    public static class MessageMappings
    {
        public static void Initialize()
        {
            MapMessageTypes();
            MapMessageTransformations();
        }
        
        private static void MapMessageTypes()
        {
            // Configure integration events - local
            AddCustomMap<PaymentFailedIntegrationEvent>(nameof(PaymentFailedIntegrationEvent));
            AddCustomMap<PaymentSucceededIntegrationEvent>(nameof(PaymentSucceededIntegrationEvent));
            
            // Configure integration events - remote
            AddCustomMap<OrderFinalizedIntegrationEvent>(nameof(OrderFinalizedIntegrationEvent));
        }
        
        private static void MapMessageTransformations()
        {

        }
    }
}