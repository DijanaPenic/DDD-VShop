using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.Infrastructure.Commands.Handlers
{
    internal class DeleteShoppingCartCommandHandler : ICommandHandler<DeleteShoppingCartCommand>
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
        
        public DeleteShoppingCartCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
            => _shoppingCartStore = shoppingCartStore;

        public async Task<Result> HandleAsync
        (
            DeleteShoppingCartCommand command,
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

            Result deleteResult = shoppingCart.Delete();
            if (deleteResult.IsError) return deleteResult.Error;

            await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return Result.Success;
        }
    }
}