using MediatR;

namespace VShop.SharedKernel.Infrastructure.Commands.Contracts
{
    public interface ICommandDecorator<in TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse> 
        where TCommand : IRequest<TResponse>
    {
        
    }
}