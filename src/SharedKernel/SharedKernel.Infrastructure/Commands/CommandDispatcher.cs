using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

namespace VShop.SharedKernel.Infrastructure.Commands
{
    internal class CommandDispatcher: ICommandDispatcher
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

        public async Task<Result> SendAsync
        (
            ICommand command,
            CancellationToken cancellationToken
        )
        {
            _contextAdapter.ChangeContext(command);
        
            using IServiceScope scope = _serviceProvider.CreateScope();
            
            Type handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            object handler = scope.ServiceProvider.GetRequiredService(handlerType);
            MethodInfo method = handlerType.GetMethod(nameof(ICommandHandler<ICommand>.HandleAsync));
        
            if (method is null) throw new InvalidOperationException("Command handler is invalid.");
        
            // ReSharper disable once PossibleNullReferenceException
            return await (Task<Result>)method.Invoke(handler, new object[] {command, cancellationToken});
        }

        public async Task<Result<TResponse>> SendAsync<TResponse>
        (
            ICommand<TResponse> command,
            CancellationToken cancellationToken
        )
        {
            _contextAdapter.ChangeContext(command);
        
            using IServiceScope scope = _serviceProvider.CreateScope();
            
            Type handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
            object handler = scope.ServiceProvider.GetRequiredService(handlerType);
            MethodInfo method = handlerType.GetMethod(nameof(ICommandHandler<ICommand<TResponse>, TResponse>.HandleAsync));
        
            if (method is null) throw new InvalidOperationException("Command handler is invalid.");
        
            // ReSharper disable once PossibleNullReferenceException
            return await (Task<Result<TResponse>>)method.Invoke(handler, new object[] {command, cancellationToken});
        }
    }
}