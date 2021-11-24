using MediatR;

namespace VShop.SharedKernel.Application.Decorators.Contracts
{
    public interface ICommandDecorator<in TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
    {
        
    }
}