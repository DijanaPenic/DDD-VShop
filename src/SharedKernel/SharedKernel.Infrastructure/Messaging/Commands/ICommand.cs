using OneOf;
using MediatR;

using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Infrastructure.Messaging.Commands
{
    public interface ICommand<TResult> : ICommand, IRequest<OneOf<TResult, ApplicationError>>
    {
	
    }
    
    public interface ICommand : IMessage
    {
	
    }
}