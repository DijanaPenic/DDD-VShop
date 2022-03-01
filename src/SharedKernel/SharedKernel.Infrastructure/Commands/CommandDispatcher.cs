using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Commands;

internal class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IContextAdapter _contextAdapter;

    public CommandDispatcher
    (
        IServiceProvider serviceProvider,
        IContextAdapter contextAdapter
    )
    {
        _serviceProvider = serviceProvider;
        _contextAdapter = contextAdapter;
    }

    public Task<Result> SendAsync
    (
        ICommand command,
        CancellationToken cancellationToken
    )
    {
        if (command is null) throw new ArgumentNullException(nameof(command));
        
        Type handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        
        return InvokeHandlerAsync<Result>(command, handlerType, cancellationToken);
    }

    public Task<Result<TResponse>> SendAsync<TResponse>
    (
        ICommand<TResponse> command,
        CancellationToken cancellationToken
    )
    {
        if (command is null) throw new ArgumentNullException(nameof(command));
        
        Type handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        
        return InvokeHandlerAsync<Result<TResponse>>(command, handlerType, cancellationToken);
    }
    
    private async Task<T> InvokeHandlerAsync<T>
    (
        object command,
        Type handlerType,
        CancellationToken cancellationToken
    )
    {
        if (command is null) throw new ArgumentNullException(nameof(command));

        _contextAdapter.ChangeContext((IMessage) command);
    
        using IServiceScope scope = _serviceProvider.CreateScope();
    
        object handler = scope.ServiceProvider.GetRequiredService(handlerType);
        MethodInfo method = handlerType.GetMethod("HandleAsync");
    
        if (method is null) throw new InvalidOperationException("Command handler is invalid.");
    
        // ReSharper disable once PossibleNullReferenceException
        return await (Task<T>)method.Invoke(handler, new[] {command, cancellationToken});
    }
}