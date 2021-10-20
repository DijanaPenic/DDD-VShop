using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.Services.Basket.Domain.Models.Shared;
using VShop.Services.Basket.API.Application.Commands.Shared;

namespace VShop.Services.Basket.API.Application.Commands
{
    public class CreateBasketCommandHandler : IRequestHandler<CreateBasketCommand, bool>
    {
        private readonly IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> _basketRepository;
        
        public CreateBasketCommandHandler(IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> basketRepository)
        {
            _basketRepository = basketRepository;
        }
        
        public async Task<bool> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
        {
            Domain.Models.BasketAggregate.Basket basket = Domain.Models.BasketAggregate.Basket.Create
            (
                EntityId.Create(command.CustomerId), 
                command.CustomerDiscount
            );

            foreach (BasketItemDto basketItem in command.BasketItems)
            {
                basket.AddProduct
                (
                    EntityId.Create(basketItem.ProductId), 
                    ProductQuantity.Create(basketItem.Quantity), 
                    Price.Create(basketItem.UnitPrice)
                );
            }
            
            await _basketRepository.SaveAsync(basket);

            return true;
        }
    }
}