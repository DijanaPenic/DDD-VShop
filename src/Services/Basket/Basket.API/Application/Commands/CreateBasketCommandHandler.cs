using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace VShop.Services.Basket.API.Application.Commands
{
   
    public class CreateBasketCommandHandler : IRequestHandler<CreateBasketCommand, bool>
    {
        public CreateBasketCommandHandler()
        {
        }
        
        public Task<bool> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}