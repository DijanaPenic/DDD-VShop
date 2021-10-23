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
        private readonly IEventStoreAggregateRepository<Domain.Models.ShoppingCartAggregate.ShoppingCart, EntityId> _basketRepository;
        
        public CreateShoppingCartCommandHandler(IEventStoreAggregateRepository<Domain.Models.ShoppingCartAggregate.ShoppingCart, EntityId> basketRepository)
        {
            _basketRepository = basketRepository;
        }
        
        public async Task<OneOf<Success<Domain.Models.ShoppingCartAggregate.ShoppingCart>, ApplicationError>> Handle(CreateShoppingCartCommand command, CancellationToken cancellationToken)
        {
            Domain.Models.ShoppingCartAggregate.ShoppingCart basket = Domain.Models.ShoppingCartAggregate.ShoppingCart.Create
            (
                EntityId.Create(command.CustomerId),
                command.CustomerDiscount
            );

            foreach (BasketItemDto basketItem in command.BasketItems)
            {
                Option<ApplicationError> errorResult = basket.AddProduct
                (
                    EntityId.Create(basketItem.ProductId),
                    ProductQuantity.Create(basketItem.Quantity),
                    Price.Create(basketItem.UnitPrice)
                );

                if (errorResult.IsSome(out ApplicationError error)) return error;
            }
            
            await _basketRepository.SaveAsync(basket);

            return new Success<Domain.Models.ShoppingCartAggregate.ShoppingCart>(basket);
        }
    }
}