﻿using Polly;
using Polly.Retry;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Application.Decorators
{
    public class RetryPolicyCommandDecorator<TRequest, TResponse> : ICommandDecorator<TRequest, TResponse>
    {
        private static readonly ILogger Logger = Log.ForContext<LoggingCommandDecorator<TRequest, TResponse>>();
        
        public async Task<Result<TResponse>> Handle
        (
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<Result<TResponse>> next
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
                            "Failed to execute handler for request {Request}, retrying after {RetryTimeSpan}s: {ExceptionMessage}",
                            typeof(TRequest).Name, ts.TotalSeconds, ex.Message
                        )
                );

            return await retryPolicy.ExecuteAsync(() => next());
        }
    }
}