using OneOf;
using MediatR;

using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Infrastructure.Messaging.Commands.Publishing
{
    public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, OneOf<TResult, ApplicationError>>
        where TCommand : ICommand<TResult>
    {
	
    }
}