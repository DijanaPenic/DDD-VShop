using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Helpers
{
    internal static class ShoppingCartHelper
    {
        public static Task<ShoppingCart> GetShoppingCartAsync(EntityId shoppingCartId)
            => IntegrationTestsFixture.SalesModule.ExecuteServiceAsync<IAggregateStore<ShoppingCart>, ShoppingCart>
               (store => store.LoadAsync(shoppingCartId));
        
        public static Task SaveAndPublishAsync(ShoppingCart shoppingCart)
            => IntegrationTestsFixture.SalesModule.ExecuteServiceAsync<IAggregateStore<ShoppingCart>>
               (store => store.SaveAndPublishAsync(shoppingCart));
        
        public static Task SaveAsync(ShoppingCart shoppingCart)
            => IntegrationTestsFixture.SalesModule.ExecuteServiceAsync<IAggregateStore<ShoppingCart>>
            (store => store.SaveAsync(shoppingCart));
    }
}