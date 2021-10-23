using OneOf;
using OneOf.Types;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Domain.ValueObjects;

namespace VShop.Services.ShoppingCarts.API.Application.Commands
{
    public class RemoveShoppingCartProductCommandHandler : ICommandHandler<RemoveShoppingCartProductCommand, Success>
    {
        private readonly IEventStoreAggregateRepository<Domain.Models.ShoppingCartAggregate.ShoppingCart, EntityId> _shoppingCartRepository;
        
        public RemoveShoppingCartProductCommandHandler(IEventStoreAggregateRepository<Domain.Models.ShoppingCartAggregate.ShoppingCart, EntityId> shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }
        
        public async Task<OneOf<Success, ApplicationError>> Handle(RemoveShoppingCartProductCommand command, CancellationToken cancellationToken)
        {
            Domain.Models.ShoppingCartAggregate.ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));
            if (shoppingCart is null) return NotFoundError.Create("Shopping cart not found.");
            
            Option<ApplicationError> errorResult = shoppingCart.RemoveProduct(EntityId.Create(command.ProductId));
            
            if (errorResult.IsSome(out ApplicationError error)) return error;

            await _shoppingCartRepository.SaveAsync(shoppingCart);

            return new Success();
        }
    }
}