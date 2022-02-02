using Microsoft.Extensions.Hosting;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Tests.IntegrationTests.Probing.Contracts;

namespace VShop.SharedKernel.Tests.IntegrationTests.Contracts;

public interface IModuleFixture
{
    IMessageRegistry MessageRegistry { get; }
    string RelationalDbConnectionString { get; }
    Task AssertEventuallyAsync(IClockService clockService, IProbe probe, int timeout);
    Task ExecuteHostedServiceAsync(Func<IHostedService, Task> action, string hostedServiceName);
    Task ExecuteHostedServiceAsync<TService>(Func<TService, Task> action) where TService : IHostedService;
    Task ExecuteServiceAsync<TService>(Func<TService, Task> action);
    Task<TResult> ExecuteServiceAsync<TService, TResult>(Func<TService, Task<TResult>> action);
    Task<Result> SendAsync(ICommand command, IContext context = default);
    Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, IContext context = default);
    Task PublishAsync(IIntegrationEvent @event, IContext context = default);
}