using Microsoft.EntityFrameworkCore;

using VShop.Modules.Catalog.Infrastructure.DAL;
using VShop.Modules.Catalog.Infrastructure.DAL.Entities;
using VShop.Modules.Catalog.Integration.Events;
using VShop.Modules.Sales.Integration.Events;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;

namespace VShop.Modules.Catalog.Infrastructure.Events.Handlers
{
    internal class OrderPaidIntegrationEventHandler : IEventHandler<OrderStatusSetToPaidIntegrationEvent>
    {
        private readonly CatalogDbContext _catalogDbContext;
        private readonly IIntegrationEventService _integrationEventService;

        public OrderPaidIntegrationEventHandler
        (
            CatalogDbContext catalogDbContext,
            IIntegrationEventService integrationEventService
        )
        {
            _catalogDbContext = catalogDbContext;
            _integrationEventService = integrationEventService;
        }

        public async Task HandleAsync(OrderStatusSetToPaidIntegrationEvent @event, CancellationToken cancellationToken)
        {
            IList<OrderStockProcessedIntegrationEvent.Types.OrderLine> confirmedOrderLines = 
                new List<OrderStockProcessedIntegrationEvent.Types.OrderLine>();

            foreach (OrderStatusSetToPaidIntegrationEvent.Types.OrderLine orderLine in @event.OrderLines)
            {
                CatalogProduct product = await _catalogDbContext.Products
                    .SingleOrDefaultAsync(p => p.Id == orderLine.ProductId, cancellationToken);

                if (product is null) return;
                
                Result<int> decreaseStockResult = product.DecreaseStock(orderLine.Quantity);
                if (decreaseStockResult.IsError) return;
                
                confirmedOrderLines.Add(new OrderStockProcessedIntegrationEvent.Types.OrderLine
                (
                    orderLine.ProductId,
                    orderLine.Quantity,
                    orderLine.Quantity - decreaseStockResult.Data
                ));
            }

            OrderStockProcessedIntegrationEvent orderStockConfirmedIntegrationEvent = new
            (
                @event.OrderId,
                confirmedOrderLines
            );
            
            await _integrationEventService.SaveEventAsync
            (
                orderStockConfirmedIntegrationEvent,
                cancellationToken
            );
        }
    }
}