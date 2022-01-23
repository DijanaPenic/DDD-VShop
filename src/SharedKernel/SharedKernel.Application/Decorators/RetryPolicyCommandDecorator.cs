using Polly;
using Polly.Retry;
using MediatR;
using Serilog;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.SharedKernel.Application.Decorators
{
    public class RetryPolicyCommandDecorator<TCommand, TResponse> : ICommandDecorator<TCommand, TResponse>
        where TCommand : IRequest<TResponse>
        where TResponse : IResult
    {
        private readonly ILogger _logger;

        public RetryPolicyCommandDecorator(ILogger logger) => _logger = logger;
        
        public async Task<TResponse> Handle
        (
            TCommand command,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next
        )
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

            return await retryPolicy.ExecuteAsync(() => next());
        }
    }
}