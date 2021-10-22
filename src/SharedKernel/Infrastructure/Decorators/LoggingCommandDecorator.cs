using OneOf;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Commands;

namespace VShop.SharedKernel.Infrastructure.Decorators
{
    public class LoggingCommandDecorator<TRequest, TResponse> : ICommandDecorator<TRequest, TResponse>
    {
        private static readonly ILogger Logger = Log.ForContext<LoggingCommandDecorator<TRequest, TResponse>>();
        
        public async Task<OneOf<TResponse, ApplicationError>> Handle
        (
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<OneOf<TResponse, ApplicationError>> next
        )
        {
            string requestTypeName = request.GetType().Name;
            
            Logger.Information("----- Handling command {CommandName} ({@Command})", requestTypeName, request);
            
            OneOf<TResponse, ApplicationError> response = await next();
            
            Logger.Information("----- Command {CommandName} handled - response: {@Response}", requestTypeName, response);

            return response;
        }
    }
}