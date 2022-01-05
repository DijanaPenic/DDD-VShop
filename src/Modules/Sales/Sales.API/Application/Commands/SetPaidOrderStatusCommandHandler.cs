using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Sales.Domain.Models.Ordering;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class SetPaidOrderStatusCommandHandler : ICommandHandler<SetPaidOrderStatusCommand>
    {
        private readonly IAggregateStore<Order> _orderStore;

        public SetPaidOrderStatusCommandHandler(IAggregateStore<Order> orderStore)
            => _orderStore = orderStore;

        public async Task<Result> Handle(SetPaidOrderStatusCommand command, CancellationToken cancellationToken)
        {
            Order order = await _orderStore.LoadAsync
            (
                EntityId.Create(command.OrderId).Data,
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (order is null) return Result.NotFoundError("Order not found.");

            if (order.Events.Count is 0)
            {
                Result statusChangeResult = order.SetPaidStatus();
                if (statusChangeResult.IsError) return statusChangeResult.Error;
                
                OrderStatusSetToPaidIntegrationEvent orderPlacedIntegrationEvent = new()
                {
                    OrderId = order.Id,
                    OrderLines = order.OrderLines.Select(ol => new OrderStatusSetToPaidIntegrationEvent.OrderLine
                    {
                        ProductId = ol.Id,
                        Quantity = ol.Quantity,
                        Price = ol.UnitPrice
                    }).ToList()
                };
                
                order.RaiseEvent(orderPlacedIntegrationEvent);
            }

            await _orderStore.SaveAndPublishAsync(order, cancellationToken);

            return Result.Success;
        }
    }

    public record SetPaidOrderStatusCommand : Command
    {
        public Guid OrderId { get; }

        public SetPaidOrderStatusCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}