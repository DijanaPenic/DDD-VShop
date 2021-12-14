using System;
using AutoFixture;
using System.Threading.Tasks;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers
{
    public class ShoppingCartHelper
    {
        private readonly Fixture _autoFixture;

        public ShoppingCartHelper(Fixture autoFixture) => _autoFixture = autoFixture;
        
        public async Task<ShoppingCart> PrepareShoppingCartForCheckoutAsync()
        {
            ShoppingCart shoppingCart = new();

            shoppingCart.Create
            (
                _autoFixture.Create<EntityId>(),
                _autoFixture.Create<EntityId>(),
                _autoFixture.Create<Discount>()
            );
            
            while(!shoppingCart.HasMinAmountForCheckout)
            {
                shoppingCart.AddProduct
                (
                    _autoFixture.Create<EntityId>(),
                    _autoFixture.Create<ProductQuantity>(),
                    _autoFixture.Create<Price>()
                );
            };
            
            shoppingCart.Customer.SetDeliveryAddress(_autoFixture.Create<Address>());
            shoppingCart.Customer.SetContactInformation
            (
                _autoFixture.Create<FullName>(),
                _autoFixture.Create<EmailAddress>(),
                _autoFixture.Create<PhoneNumber>(),
                _autoFixture.Create<GenderType>()
            );

            await SaveShoppingCartAsync(shoppingCart);

            return shoppingCart;
        }

        public async Task<ShoppingCart> CheckoutShoppingCartAsync(IClockService clockService, Guid orderId)
        {
            ShoppingCart shoppingCart = await PrepareShoppingCartForCheckoutAsync();
            shoppingCart.RequestCheckout(clockService, EntityId.Create(orderId));
            
            await SaveShoppingCartAsync(shoppingCart);

            return shoppingCart;
        }

        public Task<ShoppingCart> GetShoppingCartAsync(Guid shoppingCartId)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateRepository<ShoppingCart, EntityId>, ShoppingCart>
                (repository => repository.LoadAsync(EntityId.Create(shoppingCartId)));
        
        public Task SaveShoppingCartAsync(ShoppingCart shoppingCart)
            => IntegrationTestsFixture.ExecuteServiceAsync<IAggregateRepository<ShoppingCart, EntityId>>
                (repository => repository.SaveAsync(shoppingCart));
    }
}