using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
            ShoppingCart shoppingCart = new()
            {
                CorrelationId = command.CorrelationId,
                CausationId = command.MessageId,
            };
            
            Result createShoppingCartResult = shoppingCart.Create
            (
                command.ShoppingCartId,
                command.CustomerId,
                command.CustomerDiscount
            );
            
            if (createShoppingCartResult.IsError) return createShoppingCartResult.Error;

            foreach (ShoppingCartItemCommandDto shoppingCartItem in command.ShoppingCartItems)
            {
                Result addProductResult = shoppingCart.AddProduct
                (
                    shoppingCartItem.ProductId,
                    shoppingCartItem.Quantity,
                    shoppingCartItem.UnitPrice
                );

                if (addProductResult.IsError) return addProductResult.Error;
            }

            await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return shoppingCart;
        }
    }
    
    public record CreateShoppingCartCommand : Command<ShoppingCart>
    {
        public EntityId ShoppingCartId { get; init; }
        public EntityId CustomerId { get; init; }
        public Discount CustomerDiscount { get; init; }
        public IList<ShoppingCartItemCommandDto> ShoppingCartItems { get; init; }
    }
}