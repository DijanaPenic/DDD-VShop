using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.API.Application.Commands
{
    public class CreateBasketCommandHandler : IRequestHandler<CreateBasketCommand, bool>
    {
        private readonly IAggregateStore _aggregateStore;
        
        public CreateBasketCommandHandler(IAggregateStore aggregateStore)
        {
            _aggregateStore = aggregateStore;
        }
        
        public async Task<bool> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
        {
            Domain.Models.BasketAggregate.Basket basket = Domain.Models.BasketAggregate.Basket.Create(new EntityId(command.CustomerId));
            await _aggregateStore.SaveAsync(basket);
            
            return true;
        }
    }
}