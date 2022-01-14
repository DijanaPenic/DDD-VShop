using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class SetShoppingCartProductPriceCommandHandler : ICommandHandler<SetShoppingCartProductPriceCommand>
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
        
        public SetShoppingCartProductPriceCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
            => _shoppingCartStore = shoppingCartStore;

        public async Task<Result> Handle
        (
            SetShoppingCartProductPriceCommand command,
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

            Result setProductPriceResult = shoppingCart.SetProductPrice
            (
                EntityId.Create(command.ProductId).Data,
                Price.Create(command.UnitPrice).Data
            );
            if (setProductPriceResult.IsError) return setProductPriceResult.Error;

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