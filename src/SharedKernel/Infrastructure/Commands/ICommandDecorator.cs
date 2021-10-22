using OneOf;
using MediatR;

using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Infrastructure.Commands
{
    public interface ICommandDecorator<TRequest, TResponse>
        : IPipelineBehavior<TRequest, OneOf<TResponse, ApplicationError>>
    {
        
    }
}