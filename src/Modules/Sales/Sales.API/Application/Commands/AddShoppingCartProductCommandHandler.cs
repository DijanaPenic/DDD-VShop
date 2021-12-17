using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class AddShoppingCartProductCommandHandler : ICommandHandler<AddShoppingCartProductCommand>
    {
        private readonly IAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;
        
        public AddShoppingCartProductCommandHandler(IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
            => _shoppingCartRepository = shoppingCartRepository;
        
        public async Task<Result> Handle(AddShoppingCartProductCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId),
                command.MessageId,
                command.CorrelationId, 
                cancellationToken
            );

            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
            
            Result addProductResult = shoppingCart.AddProduct
            (
                EntityId.Create(command.ShoppingCartItem.ProductId),
                ProductQuantity.Create(command.ShoppingCartItem.Quantity),
                Price.Create(command.ShoppingCartItem.UnitPrice)
            );
            
            if (addProductResult.IsError(out ApplicationError error)) return error;
        
            await _shoppingCartRepository.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return Result.Success;
        }
    }
    
    public record AddShoppingCartProductCommand : Command
    {
        public Guid ShoppingCartId { get; init; }
        public ShoppingCartItemCommandDto ShoppingCartItem { get; init; }
    }
}