using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Policies;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;

namespace VShop.SharedKernel.Messaging.Commands.Publishing
{
    public class CommandBus: ICommandBus
    {
        private readonly IMediator _mediator;

        public CommandBus(IMediator mediator) => _mediator = mediator;

        public Task<Result> SendAsync(Command command, CancellationToken cancellationToken = default)
            => TimeoutWrapper.ExecuteAsync((ct) => _mediator.Send(command, ct), cancellationToken);
        
        public Task<Result<TResponse>> SendAsync<TResponse>
        (
            Command<TResponse> command,
            CancellationToken cancellationToken = default
        ) => TimeoutWrapper.ExecuteAsync((ct) => _mediator.Send(command, ct), cancellationToken);

        public Task<object> SendAsync(object command, CancellationToken cancellationToken = default)
            => TimeoutWrapper.ExecuteAsync((ct) => _mediator.Send(command, ct), cancellationToken);
    }
}