using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
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
            IClockService clockService,
            ShoppingCart shoppingCart,
            Guid orderId
        )
        {
            shoppingCart.RequestCheckout(clockService, EntityId.Create(orderId));
            await ShoppingCartHelper.SaveAndPublishAsync(shoppingCart);
            
            return await GetProcessManagerAsync(orderId);
        }
        
        public static Task<OrderingProcessManager> GetProcessManagerAsync(Guid processManagerId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerRepository<OrderingProcessManager>, OrderingProcessManager>
                (repository => repository.LoadAsync(EntityId.Create(processManagerId)));
        
        public static Task<IList<IMessage>> GetProcessManagerOutboxAsync(Guid processManagerId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerRepository<OrderingProcessManager>, IList<IMessage>>
                (repository => repository.LoadOutboxAsync(EntityId.Create(processManagerId)));
        
        public static Task SaveAndPublishAsync(OrderingProcessManager processManager)
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerRepository<OrderingProcessManager>>
                (repository => repository.SaveAndPublishAsync(processManager));
        
        public static Task SaveAsync(OrderingProcessManager processManager)
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerRepository<OrderingProcessManager>>
                (repository => repository.SaveAsync(processManager));

        public static Task<Order> GetOrderAsync(Guid orderId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateRepository<Order, EntityId>, Order>
                (repository => repository.LoadAsync(EntityId.Create(orderId)));
    }
}