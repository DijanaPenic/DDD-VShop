using NodaTime;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Infrastructure.Commands;
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
            await ShoppingCartHelper.SaveAsync(shoppingCart);

            PlaceOrderCommand placeOrderCommand = new(orderId, shoppingCart.Id);
            await IntegrationTestsFixture.SalesModule.SendAsync(placeOrderCommand);

            Order order = await GetOrderAsync(orderId);
            
            return order;
        }

        public static Task<Order> GetOrderAsync(EntityId orderId)
            => IntegrationTestsFixture.SalesModule.ExecuteServiceAsync<IAggregateStore<Order>, Order>
               (store => store.LoadAsync(orderId));
    }
}