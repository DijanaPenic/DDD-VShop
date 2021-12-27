using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.Ordering;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class SetPaidOrderStatusCommandHandler : ICommandHandler<CancelOrderCommand>
    {
        private readonly IAggregateStore<Order> _orderStore;

        public SetPaidOrderStatusCommandHandler(IAggregateStore<Order> orderStore)
            => _orderStore = orderStore;

        public async Task<Result> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
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
                Result setPaidStatusResult = order.SetPaidStatus();
                if (setPaidStatusResult.IsError) return setPaidStatusResult.Error;
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