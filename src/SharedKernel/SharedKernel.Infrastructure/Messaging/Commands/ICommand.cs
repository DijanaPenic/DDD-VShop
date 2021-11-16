using OneOf;
using MediatR;
using OneOf.Types;

using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Infrastructure.Messaging.Commands
{
    public interface ICommand<TResult> : IBaseCommand, IRequest<OneOf<TResult, ApplicationError>>
    {
	
    }
    
    public interface ICommand : IBaseCommand, IRequest<None>
    {
	
    }
}