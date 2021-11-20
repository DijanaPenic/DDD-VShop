using MediatR;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Messaging.Commands
{
    public abstract record Command<TData> : Message, ICommand<TData>
    {
        
    }

    public abstract record Command : Message, ICommand
    {
        
    }
    
    public interface ICommand<TData> : IBaseCommand, IRequest<Result<TData>>
    {
	
    }
    
    public interface ICommand : IBaseCommand, IRequest<Result>
    {
	
    }
}