using MediatR;

namespace VShop.SharedKernel.Application.Decorators
{
    public interface ICommandDecorator<in TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
    {
        
    }
}