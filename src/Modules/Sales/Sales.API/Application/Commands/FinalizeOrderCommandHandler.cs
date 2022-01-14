using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Sales.Domain.Models.Ordering;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class FinalizeOrderCommandHandler : ICommandHandler<FinalizeOrderCommand>
    {
        private readonly IAggregateStore<Order> _orderStore;

        public FinalizeOrderCommandHandler(IAggregateStore<Order> orderStore) => _orderStore = orderStore;

        public async Task<Result> Handle
        (
            FinalizeOrderCommand command,
            CancellationToken cancellationToken
        )
        {
            Order order = await _orderStore.LoadAsync
            (
                EntityId.Create(command.OrderId).Data,
                command.Metadata.MessageId,
                cancellationToken
            );
            
            if (order is null) return Result.NotFoundError("Order not found.");
            if (order.IsRestored) return Result.Success;

            decimal initialPaymentAmount = order.FinalAmount;
                
            // Remove out of stock items from the order.
            IEnumerable<FinalizeOrderCommand.Types.OrderLine> outOfStockOrderLines = command.OrderLines
                .Where(ol => ol.OutOfStockQuantity > 0);
            
            foreach (FinalizeOrderCommand.Types.OrderLine outOfStockOrderLine in outOfStockOrderLines)
            {
                Result removeOutOfStockResult = order.RemoveOutOfStockItems
                (
                    EntityId.Create(outOfStockOrderLine.ProductId).Data,
                    ProductQuantity.Create(outOfStockOrderLine.OutOfStockQuantity).Data
                );
                if (removeOutOfStockResult.IsError) return removeOutOfStockResult.Error;
            }

            if (order.TotalOrderLineCount > 0)
            {
                // The order is ready for shipping. 
                Result statusChangeResult = order.SetPendingShippingStatus();
                if (statusChangeResult.IsError) return statusChangeResult.Error;
            }
            else
            {
                // Nothing to ship so cancelling the order.
                Result cancelOrderResult = order.SetCancelledStatus();
                if (cancelOrderResult.IsError) return cancelOrderResult.Error;
            }
            
            // Check the payment changes (for the refund).
            decimal finalPaymentAmount = order.FinalAmount;
            decimal deltaPaymentAmount = initialPaymentAmount - finalPaymentAmount;

            OrderFinalizedIntegrationEvent orderFinalizedIntegrationEvent = new
            (
                orderId: order.Id,
                // Delivery costs need to be refunded as well if there is nothing to deliver.
                refundAmount: (order.TotalOrderLineCount > 0) ? deltaPaymentAmount : initialPaymentAmount,
                orderLines: order.OrderLines
                    .Select(ol => new OrderFinalizedIntegrationEvent.Types.OrderLine
                    (
                        ol.Id,
                        ol.Quantity
                    )).ToList()
            );
            
            order.RaiseEvent(orderFinalizedIntegrationEvent);
            
            // TODO - will need to send an email to customer.

            await _orderStore.SaveAndPublishAsync
            (
                order,
                command.Metadata.MessageId,
                command.Metadata.CorrelationId,
                cancellationToken
            );

            return Result.Success;
        }
    }
}