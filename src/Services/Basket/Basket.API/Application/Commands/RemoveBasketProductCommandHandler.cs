using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.Infrastructure.Domain.ValueObjects;

namespace VShop.Services.Basket.API.Application.Commands
{
    public class RemoveBasketProductCommandHandler : IRequestHandler<RemoveBasketProductCommand, bool>
    {
        private readonly IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> _basketRepository;
        
        public RemoveBasketProductCommandHandler(IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> basketRepository)
        {
            _basketRepository = basketRepository;
        }
        
        public async Task<bool> Handle(RemoveBasketProductCommand command, CancellationToken cancellationToken)
        {
            Domain.Models.BasketAggregate.Basket basket = await _basketRepository.LoadAsync(EntityId.Create(command.BasketId));
            basket.RemoveProduct(EntityId.Create(command.ProductId));

            await _basketRepository.SaveAsync(basket);

            return true;
        }
    }
}