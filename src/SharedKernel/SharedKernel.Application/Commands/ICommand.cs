using OneOf;
using MediatR;

using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Application.Commands
{
    // TODO - probably move to infrastructure>messaging
    // TODO - add IMessage interface
    public interface ICommand<TResult> : IRequest<OneOf<TResult, ApplicationError>>
    { 
	
    }
}