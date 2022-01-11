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

        public async Task<Result<ShoppingCart>> Handle
        (
            IdentifiedCommand<CreateShoppingCartCommand, ShoppingCart> command,
            CancellationToken cancellationToken
        )
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                EntityId.Create(command.Data.ShoppingCartId).Data, // TODO - improve validation in commands.
                command.Metadata.MessageId,
                command.Metadata.CorrelationId,
                cancellationToken
            );
            
            if (shoppingCart is null)
            {
                Result<ShoppingCart> createShoppingCartResult = ShoppingCart.Create
                (
                    EntityId.Create(command.Data.ShoppingCartId).Data,
                    EntityId.Create(command.Data.CustomerId).Data,
                    Discount.Create(command.Data.CustomerDiscount).Data,
                    command.Metadata.MessageId,
                    command.Metadata.CorrelationId
                );
                if (createShoppingCartResult.IsError) return createShoppingCartResult.Error;

                shoppingCart = createShoppingCartResult.Data;
                foreach (ShoppingCartItemCommand shoppingCartItem in command.Data.ShoppingCartItems)
                {
                    Result addProductResult = shoppingCart.AddProductQuantity
                    (
                        EntityId.Create(shoppingCartItem.ProductId).Data,
                        ProductQuantity.Create(shoppingCartItem.Quantity).Data,
                        Price.Create(shoppingCartItem.UnitPrice).Data
                    );
                    if (addProductResult.IsError) return addProductResult.Error;
                }
            }
            
            await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return shoppingCart;
        }
    }
}