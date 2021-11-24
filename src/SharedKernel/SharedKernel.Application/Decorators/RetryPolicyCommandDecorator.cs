using Polly;
using Polly.Retry;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;

using VShop.SharedKernel.Application.Decorators.Contracts;

namespace VShop.SharedKernel.Application.Decorators
{
    public class RetryPolicyCommandDecorator<TCommand, TResponse> : ICommandDecorator<TCommand, TResponse>
    {
        private static readonly ILogger Logger = Log.ForContext<RetryPolicyCommandDecorator<TCommand, TResponse>>();
        
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
                .Handle<Exception>() // TODO - which exceptions should be retried?
                .WaitAndRetryAsync
                (
                    maxRetryAttempts,
                    provider => TimeSpan.FromMilliseconds(provider * sleepDuration),
                    (ex, ts, _)
                        => Logger.Warning
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