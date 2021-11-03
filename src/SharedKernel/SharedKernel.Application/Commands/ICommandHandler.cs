using OneOf;
using MediatR;

using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.Application.Commands
{
    public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, OneOf<TResult, ApplicationError>>
        where TCommand : ICommand<TResult>
    {
	
    }
}