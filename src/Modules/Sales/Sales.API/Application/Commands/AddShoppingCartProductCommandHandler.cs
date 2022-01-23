using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.SharedKernel.Infrastructure.Commands.Publishing.Contracts;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class AddShoppingCartProductCommandHandler : ICommandHandler<AddShoppingCartProductCommand>
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
        
        public AddShoppingCartProductCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
            => _shoppingCartStore = shoppingCartStore;

        public async Task<Result> Handle
        (
            AddShoppingCartProductCommand command,
            CancellationToken cancellationToken
        )
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId).Data,
                command.Metadata.MessageId,
                cancellationToken
            );
            
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
            if (shoppingCart.IsRestored) return Result.Success;
            
            Result addProductResult = shoppingCart.AddProductQuantity
            (
                EntityId.Create(command.ShoppingCartItem.ProductId).Data,
                ProductQuantity.Create(command.ShoppingCartItem.Quantity).Data,
                Price.Create(command.ShoppingCartItem.UnitPrice.DecimalValue).Data
            );
            if (addProductResult.IsError) return addProductResult.Error;
        
            await _shoppingCartStore.SaveAndPublishAsync
            (
                shoppingCart,
                command.Metadata.MessageId,
                command.Metadata.CorrelationId,
                cancellationToken
            );

            return Result.Success;
        }
    }
}