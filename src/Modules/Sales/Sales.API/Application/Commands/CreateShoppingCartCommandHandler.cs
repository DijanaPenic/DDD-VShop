using OneOf;
using OneOf.Types;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Application.Commands;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.Modules.Sales.Domain.Models.ShoppingCartAggregate;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class CreateShoppingCartCommandHandler : ICommandHandler<CreateShoppingCartCommand, Success<ShoppingCart>>
    {
        private readonly IEventStoreAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;
        
        public CreateShoppingCartCommandHandler(IEventStoreAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }
        
        public async Task<OneOf<Success<ShoppingCart>, ApplicationError>> Handle(CreateShoppingCartCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = ShoppingCart.Create
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

            return new Success<ShoppingCart>(shoppingCart);
        }
    }
}