using MediatR;
using Serilog;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;

namespace VShop.SharedKernel.Application.Decorators
{
    public class LoggingCommandDecorator<TCommand, TResponse> : ICommandDecorator<TCommand, TResponse>
        where TCommand : IRequest<TResponse>
        where TResponse : IResult
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
            // TODO - Temp. fix
            if (command is IBaseQuery) return await next();
            
            string commandTypeName = command.GetType().Name;

            _logger.Information
            (
                "Handling command {CommandName} ({@Command})",
                commandTypeName, command
            );
            
            TResponse result = await next();

            _logger.Information
            (
                "Command {CommandName} handled; response: {@Result}",
                commandTypeName, result.ToString()
            );

            return result;
        }
    }
}