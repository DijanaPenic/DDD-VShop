using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Policies;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.SharedKernel.Infrastructure.Commands
{
    public class CommandDispatcher: ICommandDispatcher
    {
        private readonly IMediator _mediator;

        public CommandDispatcher(IMediator mediator) => _mediator = mediator;

        public Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default)
            => TimeoutWrapper.ExecuteAsync((ct) => _mediator.Send(command, ct), cancellationToken);
        
        public Task<Result<TResponse>> SendAsync<TResponse>
        (
            ICommand<TResponse> command,
            CancellationToken cancellationToken = default
        ) => TimeoutWrapper.ExecuteAsync((ct) => _mediator.Send(command, ct), cancellationToken);

        public Task<object> SendAsync(object command, CancellationToken cancellationToken = default)
            => TimeoutWrapper.ExecuteAsync((ct) => _mediator.Send(command, ct), cancellationToken);
    }
}