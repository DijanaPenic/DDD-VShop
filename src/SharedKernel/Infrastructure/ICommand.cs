using OneOf;
using MediatR;

namespace VShop.SharedKernel.Infrastructure
{
    public interface ICommand<TResult> : IRequest<OneOf<TResult, ApplicationError>> 
    { 
	
    }
}