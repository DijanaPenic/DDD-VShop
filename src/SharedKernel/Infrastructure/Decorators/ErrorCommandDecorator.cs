using OneOf;
using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Commands;

namespace VShop.SharedKernel.Infrastructure.Decorators
{
    public class ErrorCommandDecorator<TRequest, TResponse> : ICommandDecorator<TRequest, TResponse>
    {
        public async Task<OneOf<TResponse, ApplicationError>> Handle
        (
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<OneOf<TResponse, ApplicationError>> next
        )
        {
            try
            {
                OneOf<TResponse, ApplicationError> response = await next();
                
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return InternalServerError.Create(ex.InnerException?.Message);
            }
        }
    }
}