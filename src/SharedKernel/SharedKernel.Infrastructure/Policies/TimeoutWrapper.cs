using Polly;
using Polly.Timeout;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Policies
{
    // Polly Wiki page: https://github.com/App-vNext/Polly/wiki/Timeout
    public static class TimeoutWrapper
    {
        private const int TimeoutSeconds = 1000; // NOTE: this time should be decreased for production.
        
        private static readonly AsyncTimeoutPolicy TimeoutPolicy = Policy.TimeoutAsync(TimeoutSeconds);

        public static Task<TResult> ExecuteAsync<TResult>
        (
            Func<CancellationToken, Task<TResult>> action,
            CancellationToken cancellationToken = default
        )
        {
            CancellationTokenSource userCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            return TimeoutPolicy.ExecuteAsync(action, userCancellationSource.Token);
        }
    }
}