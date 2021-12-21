using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class CheckoutShoppingCartCommandHandler : ICommandHandler<CheckoutShoppingCartCommand, CheckoutOrder>
    {
        private readonly IClockService _clockService;
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;

        public CheckoutShoppingCartCommandHandler(IClockService clockService, IAggregateStore<ShoppingCart> shoppingCartStore)
        {
            _clockService = clockService;
            _shoppingCartStore = shoppingCartStore;
        }

        public async Task<Result<CheckoutOrder>> Handle(CheckoutShoppingCartCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                command.ShoppingCartId,
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
            
            // TODO - this will generate a new Id every time user submits the checkout request (problem!).
            // Potentially use the same Id for shopping cart and order.
            EntityId orderId = EntityId.Create(SequentialGuid.Create()).Data;

            Result checkoutResult = shoppingCart.RequestCheckout(orderId, _clockService.Now);
            
            if (checkoutResult.IsError) return checkoutResult.Error;

            await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);
            
            CheckoutOrder order = new() { OrderId = orderId };

            return order;
        }
    }

    public record CheckoutShoppingCartCommand : Command<CheckoutOrder>
    {
        public EntityId ShoppingCartId { get; }
        
        public CheckoutShoppingCartCommand(EntityId shoppingCartId)
        {
            ShoppingCartId = shoppingCartId;
        }
    }
    
    public record CheckoutOrder
    {
        public Guid OrderId { get; init; }
    }
}