using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Catalog.Integration.Events;

using static VShop.SharedKernel.Messaging.MessageTypeMapper;

namespace VShop.Modules.Catalog.API.Application
{
    public static class MessageMappings
    {
        public static void MapMessageTypes()
        {
            // Configure integration events - local
            AddCustomMap<OrderStockConfirmedIntegrationEvent>(nameof(OrderStockConfirmedIntegrationEvent));
            
            // Configure integration events - remote
            AddCustomMap<OrderStatusSetToPaidIntegrationEvent>(nameof(OrderStatusSetToPaidIntegrationEvent));
        }
    }
}