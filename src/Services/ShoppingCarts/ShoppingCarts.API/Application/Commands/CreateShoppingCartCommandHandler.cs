using OneOf;
using OneOf.Types;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Domain.ValueObjects;
using VShop.Services.ShoppingCarts.API.Application.Commands.Shared;

namespace VShop.Services.ShoppingCarts.API.Application.Commands
{
    public class CreateShoppingCartCommandHandler : ICommandHandler<CreateShoppingCartCommand, Success<Domain.Models.ShoppingCartAggregate.ShoppingCart>>
    {
        private readonly IEventStoreAggregateRepository<Domain.Models.ShoppingCartAggregate.ShoppingCart, EntityId> _shoppingCartRepository;
        
        public CreateShoppingCartCommandHandler(IEventStoreAggregateRepository<Domain.Models.ShoppingCartAggregate.ShoppingCart, EntityId> shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }
        
        public async Task<OneOf<Success<Domain.Models.ShoppingCartAggregate.ShoppingCart>, ApplicationError>> Handle(CreateShoppingCartCommand command, CancellationToken cancellationToken)
        {
            Domain.Models.ShoppingCartAggregate.ShoppingCart shoppingCart = Domain.Models.ShoppingCartAggregate.ShoppingCart.Create
            (
                EntityId.Create(command.CustomerId),
                command.CustomerDiscount
            );

            foreach (ShoppingCartItemDto shoppingCartItem in command.ShoppingCartItems)
            {
                Option<ApplicationError> errorResult = shoppingCart.AddProduct
                (
                    EntityId.Create(shoppingCartItem.ProductId),
                    ProductQuantity.Create(shoppingCartItem.Quantity),
                    Price.Create(shoppingCartItem.UnitPrice)
                );

                if (errorResult.IsSome(out ApplicationError error)) return error;
            }
            
            await _shoppingCartRepository.SaveAsync(shoppingCart);

            return new Success<Domain.Models.ShoppingCartAggregate.ShoppingCart>(shoppingCart);
        }
    }
}