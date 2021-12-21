using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class AddShoppingCartProductCommandHandler : ICommandHandler<AddShoppingCartProductCommand>
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
        
        public AddShoppingCartProductCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
            => _shoppingCartStore = shoppingCartStore;
        
        public async Task<Result> Handle(AddShoppingCartProductCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId).Value,
                command.MessageId,
                command.CorrelationId, 
                cancellationToken
            );

            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
            
            Result addProductResult = shoppingCart.AddProduct
            (
                EntityId.Create(command.ShoppingCartItem.ProductId).Value,
                ProductQuantity.Create(command.ShoppingCartItem.Quantity).Value,
                Price.Create(command.ShoppingCartItem.UnitPrice).Value
            );
            
            if (addProductResult.IsError) return addProductResult.Error;
        
            await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return Result.Success;
        }
    }
    
    public record AddShoppingCartProductCommand : Command
    {
        public Guid ShoppingCartId { get; init; }
        public ShoppingCartItemCommandDto ShoppingCartItem { get; init; }
    }
}