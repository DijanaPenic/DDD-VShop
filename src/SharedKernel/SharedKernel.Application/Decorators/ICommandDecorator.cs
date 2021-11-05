using OneOf;
using MediatR;

using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Application.Decorators
{
    public interface ICommandDecorator<in TRequest, TResponse>
        : IPipelineBehavior<TRequest, OneOf<TResponse, ApplicationError>>
    {
        
    }
}