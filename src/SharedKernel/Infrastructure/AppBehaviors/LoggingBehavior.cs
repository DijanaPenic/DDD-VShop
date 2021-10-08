using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace VShop.SharedKernel.Infrastructure.AppBehaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            string requestTypeName = request.GetType().FullName;
            
            _logger.LogInformation("----- Handling command {CommandName} ({@Command})", requestTypeName, request);
            
            TResponse response = await next();
            
            _logger.LogInformation("----- Command {CommandName} handled - response: {@Response}", requestTypeName, response);

            return response;
        }
    }
}