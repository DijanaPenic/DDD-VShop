using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class RemoveShoppingCartProductCommandHandler : ICommandHandler<RemoveShoppingCartProductCommand>
    {
        private readonly IAggregateRepository<ShoppingCart> _shoppingCartRepository;
        
        public RemoveShoppingCartProductCommandHandler(IAggregateRepository<ShoppingCart> shoppingCartRepository)
            => _shoppingCartRepository = shoppingCartRepository;
        
        public async Task<Result> Handle(RemoveShoppingCartProductCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId),
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
            
            Result removeProductResult = shoppingCart.RemoveProduct
            (
                EntityId.Create(command.ProductId),
                ProductQuantity.Create(command.Quantity)
            );
            
            if (removeProductResult.IsError(out ApplicationError error)) return error;

            await _shoppingCartRepository.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return Result.Success;
        }
    }
    
    public record RemoveShoppingCartProductCommand : Command
    {
        public Guid ShoppingCartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; init; }
    }
}