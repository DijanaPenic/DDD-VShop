using Force.DeepCloner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Tests.IntegrationTests.Contracts;
using VShop.SharedKernel.Tests.IntegrationTests.Probing;
using VShop.SharedKernel.Tests.IntegrationTests.Probing.Contracts;

namespace VShop.SharedKernel.Tests.IntegrationTests;

public class ModuleFixture : IModuleFixture
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly string _module;
    public IMessageRegistry MessageRegistry => _serviceProvider?.GetRequiredService<IMessageRegistry>();
    public string RelationalDbConnectionString => _configuration[$"{_module}:Postgres:ConnectionString"];

    public ModuleFixture(IServiceProvider serviceProvider, IConfiguration configuration, string module)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _module = module;
    }

    public Task StartHostedServicesAsync() => Module.StartHostedServicesAsync(_serviceProvider);

    public Task StopHostedServicesAsync() => Module.StopHostedServicesAsync(_serviceProvider);
    
    public Task AssertEventuallyAsync(IClockService clockService, IProbe probe, int timeout) 
        => new Poller(clockService, timeout).CheckAsync(probe);

    public Task ExecuteHostedServiceAsync(Func<IHostedService, Task> action, string hostedServiceName)
        => ExecuteScopeAsync(sp =>
        {
            IHostedService service = sp.GetServices<IHostedService>()
                .First(s => s.GetType().Name == hostedServiceName);

            return action(service);
        });
        
    public Task ExecuteHostedServiceAsync<TService>(Func<TService, Task> action)
        where TService : IHostedService
        => ExecuteScopeAsync(sp =>
        {
            TService service = sp.GetServices<IHostedService>().OfType<TService>().Single();
            return action(service);
        });

    public Task ExecuteServiceAsync<TService>(Func<TService, Task> action)
        => ExecuteScopeAsync(sp =>
        {
            TService service = sp.GetRequiredService<TService>();
            return action(service);
        });
        
    public Task<TResult> ExecuteServiceAsync<TService, TResult>(Func<TService, Task<TResult>> action)
        => ExecuteScopeAsync(sp =>
        {
            TService service = sp.GetRequiredService<TService>();
            return action(service);
        });
    
    public Task<Result> SendAsync(ICommand command, IContext context = default)
        => ExecuteScopeAsync(sp =>
        {
            ICommandDispatcher commandDispatcher = sp.GetRequiredService<ICommandDispatcher>();
            return commandDispatcher.SendAsync(command);
        }, context);

    public Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, IContext context = default)
        => ExecuteScopeAsync(sp =>
        {
            ICommandDispatcher commandDispatcher = sp.GetRequiredService<ICommandDispatcher>();
            return commandDispatcher.SendAsync(command);
        }, context);

    public Task PublishAsync(IIntegrationEvent @event, IContext context = default)
    {
        context ??= new Context();
        
        return ExecuteScopeAsync(sp =>
        {
            IEventDispatcher eventDispatcher = sp.GetRequiredService<IEventDispatcher>();
            IMessageContextRegistry messageContextRegistry = sp.GetRequiredService<IMessageContextRegistry>();

            messageContextRegistry.Set(@event, new MessageContext(context));

            return eventDispatcher.PublishAsync(@event);
        }, context);
    }
    
    private async Task ExecuteScopeAsync
    (
        Func<IServiceProvider, Task> action,
        IContext context = default
    )
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
            
        IContextAccessor contextAccessor = scope.ServiceProvider.GetRequiredService<IContextAccessor>();
        contextAccessor.Context = context.DeepClone();
            
        await action(scope.ServiceProvider).ConfigureAwait(false);
    }

    private async Task<TResult> ExecuteScopeAsync<TResult>
    (
        Func<IServiceProvider, Task<TResult>> action,
        IContext context = default
    )
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
            
        IContextAccessor contextAccessor = scope.ServiceProvider.GetRequiredService<IContextAccessor>();
        contextAccessor.Context = context.DeepClone();
                                      
        return await action(scope.ServiceProvider).ConfigureAwait(false);
    }
}