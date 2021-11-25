using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class CreateShoppingCartCommandHandler : ICommandHandler<CreateShoppingCartCommand, ShoppingCart>
    {
        private readonly IAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;
        
        public CreateShoppingCartCommandHandler(IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
            => _shoppingCartRepository = shoppingCartRepository;

        public async Task<Result<ShoppingCart>> Handle(CreateShoppingCartCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = new()
            {
                CorrelationId = command.CorrelationId,
                CausationId = command.MessageId,
            };
            
            Result createShoppingCartResult = shoppingCart.Create
            (
                EntityId.Create(command.ShoppingCartId),
                EntityId.Create(command.CustomerId),
                command.CustomerDiscount
            );
            
            if (createShoppingCartResult.IsError(out ApplicationError createShoppingCartError)) return createShoppingCartError;

            foreach (ShoppingCartItemCommandDto shoppingCartItem in command.ShoppingCartItems)
            {
                Result addProductResult = shoppingCart.AddProduct
                (
                    EntityId.Create(shoppingCartItem.ProductId),
                    ProductQuantity.Create(shoppingCartItem.Quantity),
                    Price.Create(shoppingCartItem.UnitPrice)
                );

                if (addProductResult.IsError(out ApplicationError addProductError)) return addProductError;
            }

            await _shoppingCartRepository.SaveAsync(shoppingCart, cancellationToken);

            return shoppingCart;
        }
    }
    
    public record CreateShoppingCartCommand : Command<ShoppingCart>
    {
        public Guid ShoppingCartId { get; init; }
        public Guid CustomerId { get; init; }
        public int CustomerDiscount { get; init; }
        public ShoppingCartItemCommandDto[] ShoppingCartItems { get; init; }
    }
}