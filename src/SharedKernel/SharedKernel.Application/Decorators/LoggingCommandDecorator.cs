using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Application.Decorators
{
    public class LoggingCommandDecorator<TRequest, TResponse> : ICommandDecorator<TRequest, TResponse>
    {
        private static readonly ILogger Logger = Log.ForContext<LoggingCommandDecorator<TRequest, TResponse>>();
        
        public async Task<Result<TResponse>> Handle
        (
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<Result<TResponse>> next
        )
        {
            string requestTypeName = request.GetType().Name;
            
            Logger.Information("Handling command {CommandName} ({@Command})", requestTypeName, request);
            
            Result<TResponse> result = await next();
            
            Logger.Information("Command {CommandName} handled - response: {@Result}", requestTypeName, result);

            return result;
        }
    }
}