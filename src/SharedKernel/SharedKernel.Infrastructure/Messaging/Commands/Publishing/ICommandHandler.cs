using OneOf;
using MediatR;
using OneOf.Types;
using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Infrastructure.Messaging.Commands.Publishing
{
    public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, OneOf<TResult, ApplicationError>>
        where TCommand : ICommand<TResult>
    {
	
    }
    
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, None>
        where TCommand : ICommand
    {
	
    }
}