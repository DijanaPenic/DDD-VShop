using MediatR;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Messaging.Commands.Publishing.Contracts
{
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
        where TCommand : IRequest<Result<TResponse>> { }
    
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
        where TCommand : IRequest<Result>  { }
}