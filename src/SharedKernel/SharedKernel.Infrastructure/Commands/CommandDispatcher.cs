using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Commands
{
    internal class CommandDispatcher: ICommandDispatcher
    {
        private readonly IMediator _mediator;
        private readonly IContextAdapter _contextAdapter;

        public CommandDispatcher
        (
            IMediator mediator,
            IContextAdapter contextAdapter
        )
        {
            _mediator = mediator;
            _contextAdapter = contextAdapter;
        }

        public Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            _contextAdapter.ChangeContext(command);
            return _mediator.Send(command, cancellationToken);
        }

        public Task<Result<TResponse>> SendAsync<TResponse>
        (
            ICommand<TResponse> command,
            CancellationToken cancellationToken = default
        )
        {
            _contextAdapter.ChangeContext(command);
            return _mediator.Send(command, cancellationToken);
        }

        public Task<object> SendAsync(object command, CancellationToken cancellationToken = default)
        {
            _contextAdapter.ChangeContext((IMessage)command);
            return _mediator.Send(command, cancellationToken);
        }
    }
}