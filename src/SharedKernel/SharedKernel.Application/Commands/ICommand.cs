using OneOf;
using MediatR;

using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.SharedKernel.Application.Commands
{
    public interface ICommand<TResult> : IRequest<OneOf<TResult, ApplicationError>>
    { 
	
    }
}