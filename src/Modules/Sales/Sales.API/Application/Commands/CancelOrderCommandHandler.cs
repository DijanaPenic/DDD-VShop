using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Models.Ordering;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
    {
        private readonly IAggregateRepository<Order, EntityId> _orderRepository;

        public CancelOrderCommandHandler(IAggregateRepository<Order, EntityId> orderRepository)
            => _orderRepository = orderRepository;

        public async Task<Result> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
        {
            Order order = await _orderRepository.LoadAsync
            (
                EntityId.Create(command.OrderId),
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (order is null) return Result.NotFoundError("Order not found.");

            Result cancelOrderResult = order.SetCancelledStatus();

            if (cancelOrderResult.IsError(out ApplicationError error)) return error;
            
            await _orderRepository.SaveAndPublishAsync(order, cancellationToken);

            return Result.Success;
        }
    }
    
    // TODO - should this be record? 
    public record CancelOrderCommand : Command
    {
        public Guid OrderId { get; }

        public CancelOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}