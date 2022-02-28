using Polly;
using Polly.Retry;
using Serilog;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.SharedKernel.Application.Decorators;

public sealed class RetryPolicyCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>, IDecorator
    where TCommand : class, ICommand
{
    private readonly ILogger _logger;
    private readonly ICommandHandler<TCommand> _handler;

    public RetryPolicyCommandHandlerDecorator(ILogger logger, ICommandHandler<TCommand> handler)
    {
        _logger = logger;
        _handler = handler;
    }
    
    public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        const int maxRetryAttempts = 3;
        const int sleepDuration = 200;
            
        AsyncRetryPolicy retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync
            (
                maxRetryAttempts,
                provider => TimeSpan.FromMilliseconds(provider * sleepDuration),
                (ex, ts, _)
                    => _logger.Warning
                    (
                        ex,
                        "Failed to execute handler for command {Command}, retrying after {RetryTimeSpan}s: {ExceptionMessage}",
                        typeof(TCommand).Name, ts.TotalSeconds, ex.Message
                    )
            );

        return await retryPolicy.ExecuteAsync(() => _handler.HandleAsync(command, cancellationToken));
    }
}

public sealed class RetryPolicyCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>, IDecorator
    where TCommand : class, ICommand<TResult>
{
    private readonly ILogger _logger;
    private readonly ICommandHandler<TCommand, TResult> _handler;

    public RetryPolicyCommandHandlerDecorator(ILogger logger, ICommandHandler<TCommand, TResult> handler)
    {
        _logger = logger;
        _handler = handler;
    }
    
    public async Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        const int maxRetryAttempts = 3;
        const int sleepDuration = 200;
            
        AsyncRetryPolicy retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync
            (
                maxRetryAttempts,
                provider => TimeSpan.FromMilliseconds(provider * sleepDuration),
                (ex, ts, _)
                    => _logger.Warning
                    (
                        ex,
                        "Failed to execute handler for command {Command}, retrying after {RetryTimeSpan}s: {ExceptionMessage}",
                        typeof(TCommand).Name, ts.TotalSeconds, ex.Message
                    )
            );

        return await retryPolicy.ExecuteAsync(() => _handler.HandleAsync(command, cancellationToken));
    }
}