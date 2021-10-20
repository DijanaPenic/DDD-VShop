using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.Services.Basket.Domain.Models.Shared;

namespace VShop.Services.Basket.API.Application.Commands
{
    public class AddBasketProductCommandHandler : IRequestHandler<AddBasketProductCommand, bool>
    {
        private readonly IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> _basketRepository;
        
        public AddBasketProductCommandHandler(IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> basketRepository)
        {
            _basketRepository = basketRepository;
        }
        
        public async Task<bool> Handle(AddBasketProductCommand command, CancellationToken cancellationToken)
        {
            Domain.Models.BasketAggregate.Basket basket = await _basketRepository.LoadAsync(EntityId.Create(command.BasketId));
            basket.AddProduct
            (
                EntityId.Create(command.BasketItem.ProductId),
                ProductQuantity.Create(command.BasketItem.Quantity),
                Price.Create(command.BasketItem.UnitPrice)
            );
        
            await _basketRepository.SaveAsync(basket);

            return true;
        }
    }
}