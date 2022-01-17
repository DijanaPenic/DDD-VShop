using MediatR;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Messaging.Commands
{
    public interface ICommand : IRequest<Result>, IBaseCommand
    {
        
    }
    
    public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
    {
        
    }
    
    public interface IBaseCommand : IMessage
    {
        
    }
}