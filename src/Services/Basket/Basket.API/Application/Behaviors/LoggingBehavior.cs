using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace VShop.Services.Basket.API.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private static readonly ILogger Logger = Log.ForContext<LoggingBehavior<TRequest, TResponse>>();

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            string requestTypeName = request.GetType().Name;
            
            Logger.Information("----- Handling command {CommandName} ({@Command})", requestTypeName, request);
            
            TResponse response = await next();
            
            Logger.Information("----- Command {CommandName} handled - response: {@Response}", requestTypeName, response);

            return response;
        }
    }
}