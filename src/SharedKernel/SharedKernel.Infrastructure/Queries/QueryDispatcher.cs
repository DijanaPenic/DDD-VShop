using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Queries.Contracts;

namespace VShop.SharedKernel.Infrastructure.Queries;

internal class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public async Task<Result<TResponse>> QueryAsync<TResponse>
    (
        IQuery<TResponse> query,
        CancellationToken cancellationToken
    )
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
            
        Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        object handler = scope.ServiceProvider.GetRequiredService(handlerType);
        MethodInfo method = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResponse>, TResponse>.HandleAsync));
        
        if (method is null) throw new InvalidOperationException("Query handler is invalid.");
        
        // ReSharper disable once PossibleNullReferenceException
        return await (Task<Result<TResponse>>)method.Invoke(handler, new object[] {query, cancellationToken});
    }
}