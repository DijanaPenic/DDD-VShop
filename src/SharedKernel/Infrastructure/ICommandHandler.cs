using OneOf;
using MediatR;

namespace VShop.SharedKernel.Infrastructure
{
    public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, OneOf<TResult, ApplicationError>>
        where TCommand : ICommand<TResult>
    {
	
    }
}