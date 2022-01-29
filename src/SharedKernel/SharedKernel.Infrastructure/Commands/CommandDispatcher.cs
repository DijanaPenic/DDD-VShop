using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Policies;
using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Commands
{
    internal class CommandDispatcher: ICommandDispatcher
    {
        private readonly IMediator _mediator;
        private readonly IMessageContextProvider _messageContextProvider;
        private readonly ContextAccessor _contextAccessor;

        public CommandDispatcher
        (
            IMediator mediator,
            IMessageContextProvider messageContextProvider,
            ContextAccessor contextAccessor
        )
        {
            _mediator = mediator;
            _messageContextProvider = messageContextProvider;
            _contextAccessor = contextAccessor;
        }

        public Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            IMessageContext messageContext = _messageContextProvider.Get(command);
            if (messageContext is not null) _contextAccessor.Context.RequestId = messageContext.MessageId;
            
            return TimeoutWrapper.ExecuteAsync((ct) => _mediator.Send(command, ct), cancellationToken);
        }

        public Task<Result<TResponse>> SendAsync<TResponse>
        (
            ICommand<TResponse> command,
            CancellationToken cancellationToken = default
        )
        {
            IMessageContext messageContext = _messageContextProvider.Get(command);
            if (messageContext is not null) _contextAccessor.Context.RequestId = messageContext.MessageId;
            
            return TimeoutWrapper.ExecuteAsync((ct) => _mediator.Send(command, ct), cancellationToken);
        }

        public Task<object> SendAsync(object command, CancellationToken cancellationToken = default)
        {
            IMessageContext messageContext = _messageContextProvider.Get((IMessage)command);
            if (messageContext is not null) _contextAccessor.Context.RequestId = messageContext.MessageId;
            
            return TimeoutWrapper.ExecuteAsync((ct) => _mediator.Send(command, ct), cancellationToken);
        }
    }
}