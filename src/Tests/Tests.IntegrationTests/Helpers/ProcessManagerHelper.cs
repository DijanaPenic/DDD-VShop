using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.Tests.IntegrationTests.Infrastructure;
using VShop.Modules.ProcessManager.Infrastructure.ProcessManagers.Ordering;

namespace VShop.Tests.IntegrationTests.Helpers;

internal static class ProcessManagerHelper
{
    public static async Task SaveProcessManagerAsync(OrderingProcessManager processManager)
    {
        // Incoming PM events are stored by remote modules.
        await IntegrationTestsFixture.ProcessManagerModule.ExecuteServiceAsync<IMessageContextRegistry>(registry =>
        {
            foreach (IBaseEvent @event in processManager.Inbox.Events)
                registry.Set(@event, new MessageContext(new Context()));
            
            return Task.CompletedTask;
        });
        
        await IntegrationTestsFixture.ProcessManagerModule.ExecuteServiceAsync<IProcessManagerStore<OrderingProcessManager>>
            (store => store.SaveAndPublishAsync(processManager, CancellationToken.None));
    }

    public static Task<OrderingProcessManager> GetOrderingProcessManagerAsync(EntityId orderId)
        => IntegrationTestsFixture.ProcessManagerModule
            .ExecuteServiceAsync<IProcessManagerStore<OrderingProcessManager>, OrderingProcessManager>
            (store => store.LoadAsync(orderId));
}