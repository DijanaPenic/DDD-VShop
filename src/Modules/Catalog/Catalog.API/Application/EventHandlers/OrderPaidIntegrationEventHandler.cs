using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Integration.Utilities;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Catalog.Infrastructure;
using VShop.Modules.Catalog.Infrastructure.Entities;
using VShop.Modules.Catalog.Integration.Events;
using VShop.Modules.Sales.Integration.Events;

namespace VShop.Modules.Catalog.API.Application.EventHandlers
{
    public class OrderPaidIntegrationEventHandler : IEventHandler<OrderStatusSetToPaidIntegrationEvent>
    {
        private readonly CatalogContext _catalogContext;
        private readonly IIntegrationEventService _catalogIntegrationEventService;

        public OrderPaidIntegrationEventHandler
        (
            CatalogContext catalogContext,
            IIntegrationEventService catalogIntegrationEventService
        )
        {
            _catalogContext = catalogContext;
            _catalogIntegrationEventService = catalogIntegrationEventService;
        }

        public async Task Handle(IdentifiedEvent<OrderStatusSetToPaidIntegrationEvent> @event, CancellationToken cancellationToken)
        {
            IList<OrderStockProcessedIntegrationEvent.OrderLine> confirmedOrderLines = 
                new List<OrderStockProcessedIntegrationEvent.OrderLine>();

            foreach (OrderStatusSetToPaidIntegrationEvent.OrderLine orderLine in @event.Data.OrderLines)
            {
                CatalogProduct product = await _catalogContext.Products
                    .SingleOrDefaultAsync(p => p.Id == orderLine.ProductId, cancellationToken);

                if (product is null) return;
                
                Result<int> decreaseStockResult = product.DecreaseStock(orderLine.Quantity);
                if (decreaseStockResult.IsError) return;
                
                confirmedOrderLines.Add(new OrderStockProcessedIntegrationEvent.OrderLine
                (
                    orderLine.ProductId,
                    orderLine.Quantity,
                    orderLine.Quantity - decreaseStockResult.Data
                ));
            }

            IIdentifiedEvent<OrderStockProcessedIntegrationEvent> orderStockConfirmedIntegrationEvent =
                new IdentifiedEvent<OrderStockProcessedIntegrationEvent>
                (
                    new OrderStockProcessedIntegrationEvent(@event.Data.OrderId, confirmedOrderLines),
                    new MessageMetadata
                    (
                        SequentialGuid.Create(),
                        @event.Metadata.CorrelationId,
                        @event.Metadata.MessageId
                    )
                );
            
            Guid transactionId = await ResilientTransaction.New(_catalogContext).ExecuteAsync(async () =>
            {
                await _catalogContext.SaveChangesAsync(cancellationToken);
                await _catalogIntegrationEventService.SaveEventAsync
                (
                    orderStockConfirmedIntegrationEvent,
                    cancellationToken
                );
            }, cancellationToken);

            await _catalogIntegrationEventService.PublishEventsAsync(transactionId, cancellationToken);
        }
    }
}