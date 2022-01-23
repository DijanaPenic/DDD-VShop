using MediatR;

namespace VShop.SharedKernel.Infrastructure.Commands
{
    public interface ICommand : IRequest<Result>, IBaseCommand
    {
        
    }
    
    public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
    {
        
    }
}