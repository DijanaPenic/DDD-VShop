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
    public class DeleteShoppingCartCommandHandler : ICommandHandler<DeleteShoppingCartCommand>
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
        
        public DeleteShoppingCartCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
            => _shoppingCartStore = shoppingCartStore;
        
        public async Task<Result> Handle(DeleteShoppingCartCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                command.ShoppingCartId,
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");

            Result deleteResult = shoppingCart.RequestDelete();
            
            if (deleteResult.IsError) return deleteResult.Error;

            await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return Result.Success;
        }
    }
    
    public record DeleteShoppingCartCommand : Command
    {
        public EntityId ShoppingCartId { get; }

        public DeleteShoppingCartCommand(EntityId shoppingCartId)
        {
            ShoppingCartId = shoppingCartId;
        }
    }
}