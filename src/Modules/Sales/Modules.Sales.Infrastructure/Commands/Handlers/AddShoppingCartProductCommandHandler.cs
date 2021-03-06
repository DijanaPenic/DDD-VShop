using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.Infrastructure.Commands.Handlers
{
    internal class AddShoppingCartProductCommandHandler : ICommandHandler<AddShoppingCartProductCommand>
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
        
        public AddShoppingCartProductCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
            => _shoppingCartStore = shoppingCartStore;

        public async Task<Result> HandleAsync
        (
            AddShoppingCartProductCommand command,
            CancellationToken cancellationToken
        )
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId).Data,
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
        
            await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return Result.Success;
        }
    }
}