using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class CreateShoppingCartCommandHandler : ICommandHandler<CreateShoppingCartCommand, ShoppingCart>
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
        
        public CreateShoppingCartCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
            => _shoppingCartStore = shoppingCartStore;

        public async Task<Result<ShoppingCart>> Handle(CreateShoppingCartCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId).Value,
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            
            if (shoppingCart is null)
            {
                shoppingCart = new ShoppingCart
                {
                    CorrelationId = command.CorrelationId,
                    CausationId = command.MessageId,
                };
            
                Result createShoppingCartResult = shoppingCart.Create
                (
                    EntityId.Create(command.ShoppingCartId).Value,
                    EntityId.Create(command.CustomerId).Value,
                    Discount.Create(command.CustomerDiscount).Value
                );
                if (createShoppingCartResult.IsError) return createShoppingCartResult.Error;

                foreach (ShoppingCartItemCommandDto shoppingCartItem in command.ShoppingCartItems)
                {
                    Result addProductResult = shoppingCart.AddProduct
                    (
                        EntityId.Create(shoppingCartItem.ProductId).Value,
                        ProductQuantity.Create(shoppingCartItem.Quantity).Value,
                        Price.Create(shoppingCartItem.UnitPrice).Value
                    );
                    if (addProductResult.IsError) return addProductResult.Error;
                }
            }
            
            await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return shoppingCart;
        }
    }
    
    public record CreateShoppingCartCommand : Command<ShoppingCart>
    {
        public Guid ShoppingCartId { get; init; }
        public Guid CustomerId { get; init; }
        public int CustomerDiscount { get; init; }
        public IList<ShoppingCartItemCommandDto> ShoppingCartItems { get; init; }
    }
}