using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.PostgresDb.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Catalog.Infrastructure;
using VShop.Modules.Catalog.Infrastructure.Entities;
using VShop.Modules.Catalog.Integration.Events;
using VShop.Modules.Sales.Integration.Events;

namespace VShop.Modules.Catalog.API.Application.EventHandlers
{
    public class OrderPaidIntegrationEventHandler : IEventHandler<OrderStatusSetToPaidIntegrationEvent>
    {
        private readonly CatalogDbContext _catalogDbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventService _catalogIntegrationEventService;

        public OrderPaidIntegrationEventHandler
        (
            CatalogDbContext catalogDbContext,
            IUnitOfWork unitOfWork,
            IIntegrationEventService catalogIntegrationEventService
        )
        {
            _catalogDbContext = catalogDbContext;
            _unitOfWork = unitOfWork;
            _catalogIntegrationEventService = catalogIntegrationEventService;
        }

        public async Task Handle(OrderStatusSetToPaidIntegrationEvent @event, CancellationToken cancellationToken)
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
                confirmedOrderLines,
                new MessageMetadata
                (
                    SequentialGuid.Create(),
                    @event.Metadata.CorrelationId,
                    @event.Metadata.MessageId
                )
            );

            // TODO - this can be done via decorator.
            Guid transactionId = await _unitOfWork.ExecuteAsync(async() =>
            {
                await _catalogDbContext.SaveChangesAsync(cancellationToken);
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