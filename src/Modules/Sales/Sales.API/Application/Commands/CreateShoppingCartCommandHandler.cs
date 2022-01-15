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
            CreateShoppingCartCommand command,
            CancellationToken cancellationToken
        )
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId).Data, // TODO - improve validation in commands.
                command.Metadata.MessageId,
                cancellationToken
            );
            
            if (shoppingCart is not null) return shoppingCart;
            
            Result<ShoppingCart> createShoppingCartResult = ShoppingCart.Create
            (
                EntityId.Create(command.ShoppingCartId).Data,
                EntityId.Create(command.CustomerId).Data,
                Discount.Create(command.CustomerDiscount).Data
            );
            if (createShoppingCartResult.IsError) return createShoppingCartResult.Error;

            shoppingCart = createShoppingCartResult.Data;
            foreach (ShoppingCartItemCommand shoppingCartItem in command.ShoppingCartItems)
            {
                Result addProductResult = shoppingCart.AddProductQuantity
                (
                    EntityId.Create(shoppingCartItem.ProductId).Data,
                    ProductQuantity.Create(shoppingCartItem.Quantity).Data,
                    Price.Create(shoppingCartItem.UnitPrice.DecimalValue).Data
                );
                if (addProductResult.IsError) return addProductResult.Error;
            }

            await _shoppingCartStore.SaveAndPublishAsync
            (
                shoppingCart,
                command.Metadata.MessageId,
                command.Metadata.CorrelationId,
                cancellationToken
            );

            return shoppingCart;
        }
    }
}