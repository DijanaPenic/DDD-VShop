using Serilog;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.SharedKernel.Application.Decorators;

public sealed class LoggingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>, IDecorator
    where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _handler;
    private readonly ILogger _logger;

    public LoggingCommandHandlerDecorator(ICommandHandler<TCommand> handler, ILogger logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        string commandTypeName = command.GetType().Name;

        _logger.Information
        (
            "Handling command {CommandName} ({@Command})",
            commandTypeName, command
        );

        Result result = await _handler.HandleAsync(command, cancellationToken);

        _logger.Information
        (
            "Command {CommandName} handled; response: {Result}",
            commandTypeName, result
        );

        return result;
    }
}

public sealed class LoggingCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>, IDecorator
    where TCommand : class, ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _handler;
    private readonly ILogger _logger;

    public LoggingCommandHandlerDecorator(ICommandHandler<TCommand, TResult> handler, ILogger logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        string commandTypeName = command.GetType().Name;

        _logger.Information
        (
            "Handling command {CommandName} ({@Command})",
            commandTypeName, command
        );

        Result<TResult> result = await _handler.HandleAsync(command, cancellationToken);

        _logger.Information
        (
            "Command {CommandName} handled; response: {Result}",
            commandTypeName, result
        );

        return result;
    }
}