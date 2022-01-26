using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Catalog.Integration.Events;

//using static VShop.SharedKernel.Infrastructure.Messaging.MessageTypeMapper;

namespace VShop.Modules.Catalog.API.Application
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
         //   AddCustomMap<OrderStockProcessedIntegrationEvent>(nameof(OrderStockProcessedIntegrationEvent));
            
            // Configure integration events - remote
         //   AddCustomMap<OrderStatusSetToPaidIntegrationEvent>(nameof(OrderStatusSetToPaidIntegrationEvent));
        }
        
        private static void MapMessageTransformations()
        {

        }
    }
}