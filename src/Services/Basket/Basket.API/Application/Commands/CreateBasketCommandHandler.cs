using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.Services.Basket.Domain.Models.Shared;

namespace VShop.Services.Basket.API.Application.Commands
{
    public class CreateBasketCommandHandler : IRequestHandler<CreateBasketCommand, bool>
    {
        private readonly IEventStoreDbRepository<Domain.Models.BasketAggregate.Basket, EntityId> _basketStore;
        
        public CreateBasketCommandHandler(IEventStoreDbRepository<Domain.Models.BasketAggregate.Basket, EntityId> basketStore)
        {
            _basketStore = basketStore;
        }
        
        public async Task<bool> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
        {
            Domain.Models.BasketAggregate.Basket basket = Domain.Models.BasketAggregate.Basket.Create
            (
                EntityId.Create(command.CustomerId), 
                command.Discount
            );
            
            await _basketStore.SaveAsync(basket);

            return true;
        }
    }
}