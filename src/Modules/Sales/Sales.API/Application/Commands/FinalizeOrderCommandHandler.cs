using System;
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

        public FinalizeOrderCommandHandler(IAggregateStore<Order> orderStore)
            => _orderStore = orderStore;

        public async Task<Result> Handle(FinalizeOrderCommand command, CancellationToken cancellationToken)
        {
            Order order = await _orderStore.LoadAsync
            (
                EntityId.Create(command.OrderId).Data,
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (order is null) return Result.NotFoundError("Order not found.");

            if (order.OutboxMessageCount is 0)
            {
                foreach (FinalizeOrderCommand.OrderLine orderLine in command.OrderLines.Where(ol => !ol.EnoughStock))
                {
                    Result removeOutOfStockResult = order.RemoveOutOfStock
                    (
                        EntityId.Create(orderLine.ProductId).Data,
                        ProductQuantity.Create(orderLine.OutOfStockQuantity).Data
                    );
                    if (removeOutOfStockResult.IsError) return removeOutOfStockResult.Error;
                }

                if (order.TotalOrderLineCount > 0)
                {
                    Result statusChangeResult = order.SetPendingShippingStatus();
                    if (statusChangeResult.IsError) return statusChangeResult.Error;
                
                    OrderStatusSetToPendingShippingIntegrationEvent orderStatusSetToPendingShippingIntegrationEvent = new()
                    {
                        OrderId = order.Id,
                        OrderLines = order.OrderLines
                            .Select(ol => new OrderStatusSetToPendingShippingIntegrationEvent.OrderLine
                            {
                                ProductId = ol.Id,
                                Quantity = ol.Quantity
                            }).ToList()
                    };
                
                    order.RaiseEvent(orderStatusSetToPendingShippingIntegrationEvent);
                }
            }

            await _orderStore.SaveAndPublishAsync(order, cancellationToken);

            return Result.Success;
        }
    }

    public record FinalizeOrderCommand : Command
    {
        public Guid OrderId { get; }
        public IList<OrderLine> OrderLines { get; }

        public FinalizeOrderCommand(Guid orderId, IList<OrderLine> orderLines)
        {
            OrderId = orderId;
            OrderLines = orderLines;
        }
        
        public record OrderLine
        {
            public Guid ProductId { get; init; }
            public int OutOfStockQuantity { get; init; }
            public bool EnoughStock { get; init; }
        }
    }
}