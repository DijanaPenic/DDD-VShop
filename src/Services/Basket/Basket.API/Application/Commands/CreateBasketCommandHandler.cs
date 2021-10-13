using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.Services.Basket.Domain.Models.Shared;

namespace VShop.Services.Basket.API.Application.Commands
{
    public class CreateBasketCommandHandler : IRequestHandler<CreateBasketCommand, bool>
    {
        private readonly IEventStoreRepository<Domain.Models.BasketAggregate.Basket, EntityId> _basketRepository;
        
        public CreateBasketCommandHandler(IEventStoreRepository<Domain.Models.BasketAggregate.Basket, EntityId> basketRepository)
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

            foreach (BasketItem basketItem in command.BasketItems)
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