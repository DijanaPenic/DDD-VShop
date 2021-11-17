using OneOf;
using OneOf.Types;
using MediatR;

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