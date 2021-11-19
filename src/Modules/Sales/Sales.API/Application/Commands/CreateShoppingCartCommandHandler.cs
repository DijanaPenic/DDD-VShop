using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using OneOf.Types;

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
    public class CreateShoppingCartCommandHandler : ICommandHandler<CreateShoppingCartCommand, Success<ShoppingCart>>
    {
        private readonly IAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;
        
        public CreateShoppingCartCommandHandler(IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
            => _shoppingCartRepository = shoppingCartRepository;

        public async Task<OneOf<Success<ShoppingCart>, ApplicationError>> Handle(CreateShoppingCartCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = ShoppingCart.Create
            (
                EntityId.Create(command.ShoppingCartId),
                EntityId.Create(command.CustomerId),
                command.CustomerDiscount,
                command.MessageId,
                command.CorrelationId
            );

            foreach (ShoppingCartItemCommandDto shoppingCartItem in command.ShoppingCartItems)
            {
                Option<ApplicationError> errorResult = shoppingCart.AddProduct
                (
                    EntityId.Create(shoppingCartItem.ProductId),
                    ProductQuantity.Create(shoppingCartItem.Quantity),
                    Price.Create(shoppingCartItem.UnitPrice)
                );

                if (errorResult.IsSome(out ApplicationError error)) return error;
            }
            
            await _shoppingCartRepository.SaveAsync(shoppingCart, cancellationToken);

            return new Success<ShoppingCart>(shoppingCart);
        }
    }
    
    public record CreateShoppingCartCommand : Command<Success<ShoppingCart>>
    {
        public Guid ShoppingCartId { get; set; }
        public Guid CustomerId { get; set; }
        public int CustomerDiscount { get; set; }
        public ShoppingCartItemCommandDto[] ShoppingCartItems { get; set; }
    }
}