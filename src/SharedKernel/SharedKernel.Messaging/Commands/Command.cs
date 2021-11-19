using OneOf;
using OneOf.Types;
using MediatR;

using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Messaging.Commands
{
    public abstract record Command<TResult> : Message, ICommand<TResult>
    {
        
    }

    public abstract record Command : Message, ICommand
    {
        
    }
    
    public interface ICommand<TResult> : IBaseCommand, IRequest<OneOf<TResult, ApplicationError>>
    {
	
    }
    
    public interface ICommand : IBaseCommand, IRequest<None>
    {
	
    }
}