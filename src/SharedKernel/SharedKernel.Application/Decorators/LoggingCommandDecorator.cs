using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Application.Decorators.Contracts;

namespace VShop.SharedKernel.Application.Decorators
{
    public class LoggingCommandDecorator<TCommand, TResponse> : ICommandDecorator<TCommand, TResponse>
    {
        private readonly ILogger _logger;

        public LoggingCommandDecorator(ILogger logger) => _logger = logger;
        
        public async Task<TResponse> Handle
        (
            TCommand command,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next
        )
        {
            string commandTypeName = command.GetType().Name;
            
            _logger.Information("Handling command {CommandName} ({@Command})", commandTypeName, command);
            
            TResponse result = await next();
            
            _logger.Information("Command {CommandName} handled - response: {@Result}", commandTypeName, result);

            return result;
        }
    }
}