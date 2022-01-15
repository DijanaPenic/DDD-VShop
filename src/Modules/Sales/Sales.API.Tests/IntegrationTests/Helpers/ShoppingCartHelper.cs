using System.Threading.Tasks;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers
{
    public static class ShoppingCartHelper
    {
        public static Task<ShoppingCart> GetShoppingCartAsync(EntityId shoppingCartId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateStore<ShoppingCart>, ShoppingCart>
               (
                    store => store.LoadAsync(shoppingCartId, SequentialGuid.Create())
               );
        
        public static Task SaveAndPublishAsync(ShoppingCart shoppingCart)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateStore<ShoppingCart>>
               (
                   store => store.SaveAndPublishAsync
                   (
                       shoppingCart,
                       SequentialGuid.Create(),
                       SequentialGuid.Create()
                   )
               );
        
        public static Task SaveAsync(ShoppingCart shoppingCart)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateStore<ShoppingCart>>
            (
                store => store.SaveAsync
                (
                    shoppingCart,
                    SequentialGuid.Create(),
                    SequentialGuid.Create()
                )
            );
    }
}