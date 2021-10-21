using OneOf;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Domain.ValueObjects;
using VShop.Services.Basket.API.Application.Commands.Shared;

namespace VShop.Services.Basket.API.Application.Commands
{
    public class CreateBasketCommandHandler : ICommandHandler<CreateBasketCommand, Domain.Models.BasketAggregate.Basket>
    {
        private readonly IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> _basketRepository;
        
        public CreateBasketCommandHandler(IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> basketRepository)
        {
            _basketRepository = basketRepository;
        }
        
        public async Task<OneOf<Domain.Models.BasketAggregate.Basket, ApplicationError>> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
        {
            Domain.Models.BasketAggregate.Basket basket = Domain.Models.BasketAggregate.Basket.Create
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

                (bool isError, ApplicationError error) = errorResult.TryGetResult();
                if (isError) return error;
            }
            
            await _basketRepository.SaveAsync(basket);

            return basket;
        }
    }
}