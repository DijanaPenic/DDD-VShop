using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using NodaTime;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.ProcessManagers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers
{
    public static class OrderHelper
    {
        public static async Task<OrderingProcessManager> PlaceOrderAsync
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            Instant now
        )
        {
            shoppingCart.RequestCheckout(orderId, now);
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            
            return await GetProcessManagerAsync(orderId);
        }
        
        public static Task<OrderingProcessManager> GetProcessManagerAsync(Guid processManagerId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerStore<OrderingProcessManager>, OrderingProcessManager>
                (store => store.LoadAsync(processManagerId));
        
        public static Task<IReadOnlyList<IMessage>> GetProcessManagerOutboxAsync(Guid processManagerId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerStore<OrderingProcessManager>, IReadOnlyList<IMessage>>
                (store => store.LoadOutboxAsync(processManagerId));
        
        public static Task SaveAndPublishAsync(OrderingProcessManager processManager)
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerStore<OrderingProcessManager>>
                (store => store.SaveAndPublishAsync(processManager));
        
        public static Task SaveAsync(OrderingProcessManager processManager)
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerStore<OrderingProcessManager>>
                (store => store.SaveAsync(processManager));

        public static Task<Order> GetOrderAsync(EntityId orderId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateStore<Order>, Order>
                (store => store.LoadAsync(orderId));
    }
}