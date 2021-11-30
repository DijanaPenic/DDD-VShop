using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class CheckoutShoppingCartCommandHandler : ICommandHandler<CheckoutShoppingCartCommand, CheckoutOrder>
    {
        private readonly IAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;

        public CheckoutShoppingCartCommandHandler(IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
            => _shoppingCartRepository = shoppingCartRepository;

        public async Task<Result<CheckoutOrder>> Handle(CheckoutShoppingCartCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId),
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
            
            // TODO - this will generate a new Id every time user submits the checkout request (problem!).
            // Potentially use the same Id for shopping cart and order.
            EntityId orderId = EntityId.Create(SequentialGuid.Create());

            Result checkoutResult = shoppingCart.RequestCheckout(orderId);
            
            if (checkoutResult.IsError(out ApplicationError error)) return error;

            await _shoppingCartRepository.SaveAsync(shoppingCart, cancellationToken);
            
            CheckoutOrder order = new() { OrderId = orderId };

            return order;
        }
    }

    public record CheckoutShoppingCartCommand : Command<CheckoutOrder>
    {
        public Guid ShoppingCartId { get; init; }
    }
    
    public record CheckoutOrder
    {
        public Guid OrderId { get; init; }
    }
}