using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.Services.Basket.Domain.Models.Shared;

namespace VShop.Services.Basket.API.Application.Commands
{
    public class AddBasketItemCommandHandler : IRequestHandler<AddBasketItemCommand, bool>
    {
        private readonly IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> _basketRepository;
        
        public AddBasketItemCommandHandler(IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> basketRepository)
        {
            _basketRepository = basketRepository;
        }
        
        public async Task<bool> Handle(AddBasketItemCommand command, CancellationToken cancellationToken)
        {
            // TODO - handling by customerId -- need to retrieve active cardholder's basket
            //
            
            Domain.Models.BasketAggregate.Basket basket = await _basketRepository.LoadAsync(EntityId.Create((Guid.Empty)));
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