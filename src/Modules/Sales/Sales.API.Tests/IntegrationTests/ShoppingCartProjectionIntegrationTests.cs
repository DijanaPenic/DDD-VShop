using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.Infrastructure.Entities;
using VShop.Modules.Sales.Tests.Customizations;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    [Collection("Non-Parallel Tests Collection")]
    public class ShoppingCartProjectionIntegrationTests : ResetDatabaseLifetime, IClassFixture<SubscriptionFixture>
    {
        [Theory]
        [CustomizedAutoData]
        public async Task Projecting_ShoppingCartCreatedDomainEvent
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart shoppingCart = new();
            shoppingCart.Create(shoppingCartId, customerId, customerDiscount);

            // Act
            await ShoppingCartHelper.SaveShoppingCartAsync(shoppingCart);
        
            // Assert
            async Task<ShoppingCartInfo> Sampling(SalesContext dbContext) 
                => await dbContext.ShoppingCarts
                    .FirstOrDefaultAsync(sc => sc.Id == shoppingCartId);

            void Validation(ShoppingCartInfo shoppingCartInfo)
            { 
                shoppingCartInfo.Should().NotBeNull();
                shoppingCartInfo!.Status.Should().Be(ShoppingCartStatus.New);
                shoppingCartInfo.CustomerId.Should().Be(customerId);
            }

            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new DatabaseProbe<SalesContext, ShoppingCartInfo>(Sampling, Validation),
                2000
            );
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Projecting_ShoppingCartProductAddedDomainEvent
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            EntityId productId,
            ProductQuantity productQuantity,
            Price productPrice
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart shoppingCart = new();
            shoppingCart.Create(shoppingCartId, customerId, customerDiscount);
            shoppingCart.AddProduct(productId, productQuantity, productPrice);

            // Act
            await ShoppingCartHelper.SaveShoppingCartAsync(shoppingCart);
        
            // Assert
            async Task<ShoppingCartInfoItem> Sampling(SalesContext dbContext) 
                => await dbContext.ShoppingCartItems
                    .FirstOrDefaultAsync(sc => sc.ShoppingCartInfoId == shoppingCartId);

            void Validation(ShoppingCartInfoItem shoppingCartInfoItem)
            { 
                shoppingCartInfoItem.Should().NotBeNull();
                shoppingCartInfoItem!.Quantity.Should().Be(productQuantity);
                shoppingCartInfoItem.ProductId.Should().Be(productId);
                shoppingCartInfoItem.UnitPrice.Should().Be(productPrice);
            }

            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new DatabaseProbe<SalesContext, ShoppingCartInfoItem>(Sampling, Validation),
                2000
            );
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Projecting_ShoppingCartProductRemovedDomainEvent
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            EntityId productId,
            ProductQuantity productQuantity,
            Price productPrice
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart shoppingCart = new();
            shoppingCart.Create(shoppingCartId, customerId, customerDiscount);
            shoppingCart.AddProduct(productId, productQuantity, productPrice);
            shoppingCart.RemoveProduct(productId, productQuantity);

            // Act
            await ShoppingCartHelper.SaveShoppingCartAsync(shoppingCart);
        
            // Assert
            async Task<ShoppingCartInfoItem> Sampling(SalesContext dbContext) 
                => await dbContext.ShoppingCartItems
                    .FirstOrDefaultAsync(sc => sc.ShoppingCartInfoId == shoppingCartId);

            void Validation(ShoppingCartInfoItem shoppingCartInfoItem) 
                => shoppingCartInfoItem.Should().BeNull();

            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new DatabaseProbe<SalesContext, ShoppingCartInfoItem>(Sampling, Validation),
                2000
            );
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Projecting_ShoppingCartDeliveryAddressSetDomainEvent
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            Address deliveryAddress
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart shoppingCart = new();
            shoppingCart.Create(shoppingCartId, customerId, customerDiscount);
            shoppingCart.Customer.SetDeliveryAddress(deliveryAddress);

            // Act
            await ShoppingCartHelper.SaveShoppingCartAsync(shoppingCart);
        
            // Assert
            async Task<ShoppingCartInfo> Sampling(SalesContext dbContext) 
                => await dbContext.ShoppingCarts
                    .FirstOrDefaultAsync(sc => sc.Id == shoppingCartId);

            void Validation(ShoppingCartInfo shoppingCartInfo)
                => shoppingCartInfo.Status.Should().Be(ShoppingCartStatus.AwaitingConfirmation);

            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new DatabaseProbe<SalesContext, ShoppingCartInfo>(Sampling, Validation),
                2000
            );
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Projecting_ShoppingCartCheckoutRequestedDomainEvent
        (
            ShoppingCart shoppingCart,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            shoppingCart.RequestCheckout(clockService, orderId);

            // Act
            await ShoppingCartHelper.SaveShoppingCartAsync(shoppingCart);
        
            // Assert
            async Task<ShoppingCartInfo> Sampling(SalesContext dbContext) 
                => await dbContext.ShoppingCarts
                    .FirstOrDefaultAsync(sc => sc.Id == shoppingCart.Id);

            void Validation(ShoppingCartInfo shoppingCartInfo)
                => shoppingCartInfo.Status.Should().Be(ShoppingCartStatus.PendingCheckout);

            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new DatabaseProbe<SalesContext, ShoppingCartInfo>(Sampling, Validation),
                2000
            );
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Projecting_ShoppingCartDeletionRequestedDomainEvent(ShoppingCart shoppingCart)
        {
            // Arrange
            IClockService clockService = new ClockService();

            shoppingCart.RequestDelete();

            // Act
            await ShoppingCartHelper.SaveShoppingCartAsync(shoppingCart);
        
            // Assert
            async Task<ShoppingCartInfo> Sampling(SalesContext dbContext) 
                => await dbContext.ShoppingCarts
                    .FirstOrDefaultAsync(sc => sc.Id == shoppingCart.Id);

            void Validation(ShoppingCartInfo shoppingCartInfo)
                => shoppingCartInfo.Status.Should().Be(ShoppingCartStatus.Closed);

            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new DatabaseProbe<SalesContext, ShoppingCartInfo>(Sampling, Validation),
                2000
            );
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Projecting_ShoppingCartItemQuantityIncreasedDomainEvent
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            EntityId productId,
            Price productPrice
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart shoppingCart = new();
            shoppingCart.Create(shoppingCartId, customerId, customerDiscount);
            shoppingCart.AddProduct(productId, ProductQuantity.Create(1), productPrice);
            shoppingCart.AddProduct(productId, ProductQuantity.Create(2), productPrice);

            // Act
            await ShoppingCartHelper.SaveShoppingCartAsync(shoppingCart);
        
            // Assert
            async Task<ShoppingCartInfoItem> Sampling(SalesContext dbContext) 
                => await dbContext.ShoppingCartItems
                    .FirstOrDefaultAsync(sc => sc.ShoppingCartInfoId == shoppingCartId);

            void Validation(ShoppingCartInfoItem shoppingCartInfoItem) 
                => shoppingCartInfoItem.Quantity.Should().Be(3);

            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new DatabaseProbe<SalesContext, ShoppingCartInfoItem>(Sampling, Validation),
                2000
            );
        }
        
        [Theory]
        [CustomizedAutoData]
        public async Task Projecting_ShoppingCartItemQuantityDecreasedDomainEvent
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            EntityId productId,
            Price productPrice
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart shoppingCart = new();
            shoppingCart.Create(shoppingCartId, customerId, customerDiscount);
            shoppingCart.AddProduct(productId, ProductQuantity.Create(10), productPrice);
            shoppingCart.RemoveProduct(productId, ProductQuantity.Create(2));

            // Act
            await ShoppingCartHelper.SaveShoppingCartAsync(shoppingCart);
        
            // Assert
            async Task<ShoppingCartInfoItem> Sampling(SalesContext dbContext) 
                => await dbContext.ShoppingCartItems
                    .FirstOrDefaultAsync(sc => sc.ShoppingCartInfoId == shoppingCartId);

            void Validation(ShoppingCartInfoItem shoppingCartInfoItem) 
                => shoppingCartInfoItem.Quantity.Should().Be(8);

            await IntegrationTestsFixture.AssertEventuallyAsync
            (
                clockService,
                new DatabaseProbe<SalesContext, ShoppingCartInfoItem>(Sampling, Validation),
                2000
            );
        }
    }
}