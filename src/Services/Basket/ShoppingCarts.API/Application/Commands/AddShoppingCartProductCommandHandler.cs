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
        private readonly IEventStoreAggregateRepository<Domain.Models.ShoppingCartAggregate.ShoppingCart, EntityId> _basketRepository;
        
        public AddShoppingCartProductCommandHandler(IEventStoreAggregateRepository<Domain.Models.ShoppingCartAggregate.ShoppingCart, EntityId> basketRepository)
        {
            _basketRepository = basketRepository;
        }
        
        public async Task<OneOf<Success, ApplicationError>> Handle(AddShoppingCartProductCommand command, CancellationToken cancellationToken)
        {
            Domain.Models.ShoppingCartAggregate.ShoppingCart basket = await _basketRepository.LoadAsync(EntityId.Create(command.BasketId));

            if (basket is null) return NotFoundError.Create("Basket not found.");
            
            Option<ApplicationError> errorResult = basket.AddProduct
            (
                EntityId.Create(command.BasketItem.ProductId),
                ProductQuantity.Create(command.BasketItem.Quantity),
                Price.Create(command.BasketItem.UnitPrice)
            );
            
            if (errorResult.IsSome(out ApplicationError error)) return error;
        
            await _basketRepository.SaveAsync(basket);

            return new Success();
        }
    }
}