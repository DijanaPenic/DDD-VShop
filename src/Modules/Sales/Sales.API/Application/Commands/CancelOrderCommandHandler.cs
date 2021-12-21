using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.Ordering;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
    {
        private readonly IAggregateStore<Order> _orderStore;

        public CancelOrderCommandHandler(IAggregateStore<Order> orderStore)
            => _orderStore = orderStore;

        public async Task<Result> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
        {
            Order order = await _orderStore.LoadAsync
            (
                EntityId.Create(command.OrderId).Value,
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (order is null) return Result.NotFoundError("Order not found.");

            Result cancelOrderResult = order.SetCancelledStatus();

            if (cancelOrderResult.IsError) return cancelOrderResult.Error;
            
            await _orderStore.SaveAndPublishAsync(order, cancellationToken);

            return Result.Success;
        }
    }
    
    public record CancelOrderCommand : Command
    {
        public Guid OrderId { get; }

        public CancelOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}