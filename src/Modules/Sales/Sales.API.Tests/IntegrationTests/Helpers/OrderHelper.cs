using System;
using System.Collections.Generic;
using AutoFixture;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.API.Application.ProcessManagers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers
{
    public class OrderHelper
    {
        private readonly Fixture _autoFixture;
        private readonly ShoppingCartHelper _shoppingCartHelper;

        public OrderHelper(Fixture autoFixture, ShoppingCartHelper shoppingCartHelper)
        {
            _autoFixture = autoFixture;
            _shoppingCartHelper = shoppingCartHelper;
        }
        
        public async Task<OrderingProcessManager> PlaceOrderAsync(IClockService clockService, Guid orderId)
        {
            await _shoppingCartHelper.CheckoutShoppingCartAsync(clockService, orderId);
            
            return await GetProcessManagerAsync(orderId);
        }
        
        public Task<OrderingProcessManager> GetProcessManagerAsync(Guid processManagerId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerRepository<OrderingProcessManager>, OrderingProcessManager>
                (repository => repository.LoadAsync(EntityId.Create(processManagerId)));
        
        public Task<IList<IMessage>> GetProcessManagerOutboxAsync(Guid processManagerId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerRepository<OrderingProcessManager>, IList<IMessage>>
                (repository => repository.LoadOutboxAsync(EntityId.Create(processManagerId)));
        
        public Task SaveProcessManagerAsync(OrderingProcessManager processManager)
            => IntegrationTestsFixture.ExecuteServiceAsync<IProcessManagerRepository<OrderingProcessManager>>
                (repository => repository.SaveAsync(processManager));

        public Task<Order> GetOrderAsync(Guid orderId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateRepository<Order, EntityId>, Order>
                (repository => repository.LoadAsync(EntityId.Create(orderId)));
    }
}