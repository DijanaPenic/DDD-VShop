using System;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.ProcessManagers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers
{
    public static class OrderHelper
    {
        public static async Task<Order> PlaceOrderAsync
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            Instant now
        )
        {
            shoppingCart.RequestCheckout(orderId, now);
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);

            Order order = await GetOrderAsync(orderId);
            
            return order;
        }
        
        public static Task<OrderingProcessManager> GetProcessManagerAsync(Guid processManagerId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerStore<OrderingProcessManager>, OrderingProcessManager>
               (
                    store => store.LoadAsync(processManagerId)
               );
        
        // public static Task<IReadOnlyList<IMessage>> GetProcessManagerOutboxAsync(Guid processManagerId)
        //     => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerStore<OrderingProcessManager>, IReadOnlyList<IMessage>>
        //        (
        //             store => store.LoadOutboxAsync(processManagerId)
        //        );
        
        public static Task SaveAndPublishAsync(OrderingProcessManager processManager)
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerStore<OrderingProcessManager>>
               (
                    store => store.SaveAndPublishAsync(processManager)
               );
        
        public static Task SaveAndPublishAsync(Order order)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateStore<Order>>
               (
                    store => store.SaveAndPublishAsync(order)
               );

        public static Task<Order> GetOrderAsync(EntityId orderId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateStore<Order>, Order>
               (
                    store => store.LoadAsync
                    (
                        orderId, 
                        SequentialGuid.Create(), 
                        SequentialGuid.Create()
                    )
               );
    }
}