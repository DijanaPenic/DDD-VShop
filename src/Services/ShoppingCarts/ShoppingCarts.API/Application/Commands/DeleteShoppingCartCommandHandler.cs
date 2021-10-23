using OneOf;
using OneOf.Types;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Application.Commands;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Services.ShoppingCarts.Domain.Models.ShoppingCartAggregate;

namespace VShop.Services.ShoppingCarts.API.Application.Commands
{
    public class DeleteShoppingCartCommandHandler : ICommandHandler<DeleteShoppingCartCommand, Success>
    {
        private readonly IEventStoreAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;
        
        public DeleteShoppingCartCommandHandler(IEventStoreAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }
        
        public async Task<OneOf<Success, ApplicationError>> Handle(DeleteShoppingCartCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));
            if (shoppingCart is null) return NotFoundError.Create("Shopping cart not found.");

            Option<ApplicationError> errorResult = shoppingCart.RequestDelete();
            
            if (errorResult.IsSome(out ApplicationError error)) return error;

            await _shoppingCartRepository.SaveAsync(shoppingCart);

            return new Success();
        }
    }
}