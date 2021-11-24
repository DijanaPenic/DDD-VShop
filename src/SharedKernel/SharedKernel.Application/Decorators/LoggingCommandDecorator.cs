using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Application.Decorators.Contracts;

namespace VShop.SharedKernel.Application.Decorators
{
    public class LoggingCommandDecorator<TCommand, TResponse> : ICommandDecorator<TCommand, TResponse>
    {
        private static readonly ILogger Logger = Log.ForContext<LoggingCommandDecorator<TCommand, TResponse>>();
        
        public async Task<TResponse> Handle
        (
            TCommand command,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next
        )
        {
            string commandTypeName = command.GetType().Name;
            
            Logger.Information("Handling command {CommandName} ({@Command})", commandTypeName, command);
            
            TResponse result = await next();
            
            Logger.Information("Command {CommandName} handled - response: {@Result}", commandTypeName, result);

            return result;
        }
    }
}