using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Policies;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Commands
{
    internal class CommandDispatcher: ICommandDispatcher
    {
        private readonly IMediator _mediator;
        private readonly IMessageContextRegistry _messageContextRegistry;
        private readonly IContextAdapter _contextAdapter;

        public CommandDispatcher
        (
            IMediator mediator,
            IMessageContextRegistry messageContextRegistry,
            IContextAdapter contextAdapter
        )
        {
            _mediator = mediator;
            _messageContextRegistry = messageContextRegistry;
            _contextAdapter = contextAdapter;
        }

        public Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            _contextAdapter.ChangeContext(_messageContextRegistry.Get(command), command.GetType());
            return TimeoutWrapper.ExecuteAsync((ct) => _mediator.Send(command, ct), cancellationToken);
        }

        public Task<Result<TResponse>> SendAsync<TResponse>
        (
            ICommand<TResponse> command,
            CancellationToken cancellationToken = default
        )
        {
            _contextAdapter.ChangeContext(_messageContextRegistry.Get(command), command.GetType());
            return TimeoutWrapper.ExecuteAsync((ct) => _mediator.Send(command, ct), cancellationToken);
        }

        public Task<object> SendAsync(object command, CancellationToken cancellationToken = default)
        {
            _contextAdapter.ChangeContext(_messageContextRegistry.Get((IMessage)command), command.GetType());
            return TimeoutWrapper.ExecuteAsync((ct) => _mediator.Send(command, ct), cancellationToken);
        }
    }
}