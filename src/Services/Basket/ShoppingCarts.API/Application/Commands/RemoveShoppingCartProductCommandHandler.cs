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
        private readonly IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> _basketRepository;
        
        public RemoveShoppingCartProductCommandHandler(IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> basketRepository)
        {
            _basketRepository = basketRepository;
        }
        
        public async Task<OneOf<Success, ApplicationError>> Handle(RemoveShoppingCartProductCommand command, CancellationToken cancellationToken)
        {
            Domain.Models.BasketAggregate.Basket basket = await _basketRepository.LoadAsync(EntityId.Create(command.BasketId));
            if (basket is null) return NotFoundError.Create("Basket not found.");
            
            Option<ApplicationError> errorResult = basket.RemoveProduct(EntityId.Create(command.ProductId));
            
            if (errorResult.IsSome(out ApplicationError error)) return error;

            await _basketRepository.SaveAsync(basket);

            return new Success();
        }
    }
}