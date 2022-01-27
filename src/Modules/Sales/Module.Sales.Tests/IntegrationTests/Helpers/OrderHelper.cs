using NodaTime;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Infrastructure.ProcessManagers;
using VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Helpers
{
    internal static class OrderHelper
    {
        public static async Task<Order> PlaceOrderAsync
        (
            ShoppingCart shoppingCart,
            EntityId orderId,
            Instant now
        )
        {
            shoppingCart.Checkout(orderId, now);
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);

            Order order = await GetOrderAsync(orderId);
            
            return order;
        }
        
        public static Task<OrderingProcessManager> LoadProcessManagerAsync
            (
                Guid processManagerId, 
                Guid causationId,
                Guid correlationId
            )
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerStore<OrderingProcessManager>, OrderingProcessManager>
               (
                    store => store.LoadAsync(processManagerId, causationId)
               );

        public static Task<Order> GetOrderAsync(EntityId orderId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateStore<Order>, Order>
               (
                    store => store.LoadAsync(orderId, SequentialGuid.Create())
               );
    }
}