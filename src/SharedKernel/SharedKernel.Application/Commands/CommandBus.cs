using MediatR;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Application.Commands
{
    public class CommandBus: ICommandBus
    {
        private readonly IMediator _mediator;

        public CommandBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> command)
        {
            return _mediator.Send(command);
        }
    }
}