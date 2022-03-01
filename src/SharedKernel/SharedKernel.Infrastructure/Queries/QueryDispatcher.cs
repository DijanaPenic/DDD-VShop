using System;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Queries.Contracts;

namespace VShop.SharedKernel.Infrastructure.Queries;

internal class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public Task<Result<TResponse>> QueryAsync<TResponse>
    (
        IQuery<TResponse> query,
        CancellationToken cancellationToken
    )
    {
        if (query is null) throw new ArgumentNullException(nameof(query));

        Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        
        return InvokeHandlerAsync<Result<TResponse>>(query, handlerType, cancellationToken);
    }

    private async Task<T> InvokeHandlerAsync<T>
    (
        object query,
        Type handlerType,
        CancellationToken cancellationToken
    )
    {
        if (query is null) throw new ArgumentNullException(nameof(query));

        using IServiceScope scope = _serviceProvider.CreateScope();
            
        object handler = scope.ServiceProvider.GetRequiredService(handlerType);
        MethodInfo method = handlerType.GetMethod("HandleAsync");
        
        if (method is null) throw new InvalidOperationException("Query handler is invalid.");
        
        // ReSharper disable once PossibleNullReferenceException
        return await (Task<T>)method.Invoke(handler, new[] {query, cancellationToken});;
    }
}