using MediatR;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Application.Decorators
{
    public interface ICommandDecorator<in TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
    {
        
    }
}