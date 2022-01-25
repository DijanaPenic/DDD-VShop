using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Sales.Domain.Models.Ordering;

namespace VShop.Modules.Sales.Infrastructure.Commands.Handlers
{
    internal class SetPaidOrderStatusCommandHandler : ICommandHandler<SetPaidOrderStatusCommand>
    {
        private readonly IAggregateStore<Order> _orderStore;

        public SetPaidOrderStatusCommandHandler(IAggregateStore<Order> orderStore)
            => _orderStore = orderStore;

        public async Task<Result> Handle
        (
            SetPaidOrderStatusCommand command,
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

            Result statusChangeResult = order.SetPaidStatus();
            if (statusChangeResult.IsError) return statusChangeResult.Error;
                
            OrderStatusSetToPaidIntegrationEvent orderPlacedIntegrationEvent = new
            (
                order.Id,
                order.OrderLines.Select(ol => new OrderStatusSetToPaidIntegrationEvent.Types.OrderLine
                (
                    ol.Id,
                    ol.Quantity,
                    ol.UnitPrice
                )).ToList()
            );
                
            order.RaiseEvent(orderPlacedIntegrationEvent);

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