using MediatR;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Messaging.Commands
{
    public abstract class Command<TResponse> : MessageContext, ICommand<TResponse>
    {
        
    }
    
    public abstract class Command : MessageContext, IRequest<Result>
    {
        
    }
    
    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {
        
    }
}