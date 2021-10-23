using OneOf;
using OneOf.Types;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Domain.ValueObjects;

namespace VShop.Services.ShoppingCarts.API.Application.Commands
{
    public class AddShoppingCartProductCommandHandler : ICommandHandler<AddShoppingCartProductCommand, Success>
    {
        private readonly IEventStoreAggregateRepository<Domain.Models.ShoppingCartAggregate.ShoppingCart, EntityId> _shoppingCartRepository;
        
        public AddShoppingCartProductCommandHandler(IEventStoreAggregateRepository<Domain.Models.ShoppingCartAggregate.ShoppingCart, EntityId> shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }
        
        public async Task<OneOf<Success, ApplicationError>> Handle(AddShoppingCartProductCommand command, CancellationToken cancellationToken)
        {
            Domain.Models.ShoppingCartAggregate.ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));

            if (shoppingCart is null) return NotFoundError.Create("Shopping cart not found.");
            
            Option<ApplicationError> errorResult = shoppingCart.AddProduct
            (
                EntityId.Create(command.ShoppingCartItem.ProductId),
                ProductQuantity.Create(command.ShoppingCartItem.Quantity),
                Price.Create(command.ShoppingCartItem.UnitPrice)
            );
            
            if (errorResult.IsSome(out ApplicationError error)) return error;
        
            await _shoppingCartRepository.SaveAsync(shoppingCart);

            return new Success();
        }
    }
}