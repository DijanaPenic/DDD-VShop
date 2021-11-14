using Polly;
using Polly.Retry;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

namespace VShop.SharedKernel.EventStore.Policies
{
    internal static class RetryWrapper
    {
        private const int MaxRetryAttempts = 3;
        private static readonly TimeSpan PauseBetweenFailures = TimeSpan.FromSeconds(2);

        private static readonly AsyncRetryPolicy RetryPolicy = Policy
            .Handle<Exception>(ex => ex is not WrongExpectedVersionException)
            .WaitAndRetryAsync(MaxRetryAttempts, _ => PauseBetweenFailures);
        
        public static Task ExecuteAsync
        (
            Func<CancellationToken, Task> action,
            CancellationToken cancellationToken = default
        ) => RetryPolicy.ExecuteAsync(action, cancellationToken);
        
        public static Task<TResult> ExecuteAsync<TResult>
        (
            Func<CancellationToken, TResult> action,
            CancellationToken cancellationToken = default
        ) 
            where TResult : IAsyncEnumerable<ResolvedEvent>
            => RetryPolicy.ExecuteAsync((ct) => Task.FromResult(action(ct)), cancellationToken);
    }
}