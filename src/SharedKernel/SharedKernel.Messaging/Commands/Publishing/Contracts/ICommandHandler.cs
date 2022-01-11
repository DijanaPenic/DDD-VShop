using MediatR;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Messaging.Commands.Publishing.Contracts
{
    public interface ICommandHandler<TCommand, TResponse> 
        : IRequestHandler<IdentifiedCommand<TCommand, TResponse>, Result<TResponse>>
        where TCommand : IBaseCommand { }
    
    public interface ICommandHandler<TCommand> 
        : IRequestHandler<IdentifiedCommand<TCommand>, Result>
        where TCommand : IBaseCommand { }
}