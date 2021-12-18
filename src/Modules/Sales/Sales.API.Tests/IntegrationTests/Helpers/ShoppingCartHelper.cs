using System;
using System.Threading.Tasks;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers
{
    public static class ShoppingCartHelper
    {
        public static Task<ShoppingCart> GetShoppingCartAsync(Guid shoppingCartId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateRepository<ShoppingCart, EntityId>, ShoppingCart>
                (repository => repository.LoadAsync(EntityId.Create(shoppingCartId)));  // TODO - should internal be visible to test projects?
        
        public static Task SaveAndPublishAsync(ShoppingCart shoppingCart)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateRepository<ShoppingCart, EntityId>>
                (repository => repository.SaveAndPublishAsync(shoppingCart));
        
        public static Task SaveAsync(ShoppingCart shoppingCart)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateRepository<ShoppingCart, EntityId>>
                (repository => repository.SaveAsync(shoppingCart));
    }
}