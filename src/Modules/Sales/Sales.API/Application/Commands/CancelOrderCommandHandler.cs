using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.SharedKernel.Infrastructure.Commands.Publishing.Contracts;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
    {
        private readonly IAggregateStore<Order> _orderStore;

        public CancelOrderCommandHandler(IAggregateStore<Order> orderStore)
            => _orderStore = orderStore;

        public async Task<Result> Handle
        (
            CancelOrderCommand command,
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

            Result cancelOrderResult = order.SetCancelledStatus();
            if (cancelOrderResult.IsError) return cancelOrderResult.Error;

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