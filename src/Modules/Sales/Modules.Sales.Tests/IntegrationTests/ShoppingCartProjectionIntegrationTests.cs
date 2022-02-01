using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Infrastructure.DAL;
using VShop.Modules.Sales.Infrastructure.DAL.Entities;
using VShop.Modules.Sales.Tests.IntegrationTests.Helpers;
using VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Services;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Tests.Customizations;
using VShop.SharedKernel.Tests.IntegrationTests.Probing;

namespace VShop.Modules.Sales.Tests.IntegrationTests
{
    [Collection("Non-Parallel Tests Collection")]
    public class ShoppingCartProjectionIntegrationTests : TestBase, IClassFixture<SubscriptionFixture>
    {
        private const int TimeoutMillis = 10000;
            
        [Theory, CustomAutoData]
        internal async Task Projecting_ShoppingCartCreatedDomainEvent_to_read_model
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            ShoppingCart shoppingCart = ShoppingCart.Create(shoppingCartId, customerId, customerDiscount).Data;
            
            // Act
            await ShoppingCartHelper.SaveAsync(shoppingCart);
        
            // Assert
            Task<ShoppingCartInfo> Sampling(SalesDbContext dbContext) 
                => dbContext.ShoppingCarts.FirstOrDefaultAsync(sc => sc.Id == shoppingCartId);

            void Validation(ShoppingCartInfo shoppingCartInfo)
            { 
                shoppingCartInfo.Should().NotBeNull();
                shoppingCartInfo!.Status.Should().Be(ShoppingCartStatus.New);
                shoppingCartInfo.CustomerId.Should().Be(customerId);
            }

            await SalesModule.AssertEventuallyAsync
            (
                clockService,
                new PostgresDatabaseProbe<SalesDbContext, ShoppingCartInfo>(SalesModule, Sampling, Validation),
                TimeoutMillis
            );
        }
        
        [Theory, CustomAutoData]
        internal async Task Projecting_ShoppingCartProductAddedDomainEvent_to_read_model
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
            
            ShoppingCart shoppingCart = ShoppingCart
                .Create(shoppingCartId, customerId, customerDiscount).Data;
            shoppingCart.AddProductQuantity(productId, productQuantity, productPrice);

            // Act
            await ShoppingCartHelper.SaveAsync(shoppingCart);
        
            // Assert
            Task<ShoppingCartInfoItem> Sampling(SalesDbContext dbContext) 
                => dbContext.ShoppingCartItems
                    .FirstOrDefaultAsync(sc => sc.ShoppingCartInfoId == shoppingCartId);

            void Validation(ShoppingCartInfoItem shoppingCartInfoItem)
            { 
                shoppingCartInfoItem.Should().NotBeNull();
                shoppingCartInfoItem!.Quantity.Should().Be(productQuantity);
                shoppingCartInfoItem.ProductId.Should().Be(productId);
                shoppingCartInfoItem.UnitPrice.Should().Be(productPrice);
            }

            await SalesModule.AssertEventuallyAsync
            (
                clockService,
                new PostgresDatabaseProbe<SalesDbContext, ShoppingCartInfoItem>(SalesModule, Sampling, Validation),
                TimeoutMillis
            );
        }
        
        [Theory, CustomAutoData]
        internal async Task Projecting_ShoppingCartProductRemovedDomainEvent_to_read_model
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
            
            ShoppingCart shoppingCart = ShoppingCart
                .Create(shoppingCartId, customerId, customerDiscount).Data;
            shoppingCart.AddProductQuantity(productId, productQuantity, productPrice);
            shoppingCart.RemoveProductQuantity(productId, productQuantity);

            // Act
            await ShoppingCartHelper.SaveAsync(shoppingCart);
        
            // Assert
            Task<ShoppingCartInfoItem> Sampling(SalesDbContext dbContext) 
                => dbContext.ShoppingCartItems
                    .FirstOrDefaultAsync(sc => sc.ShoppingCartInfoId == shoppingCartId);

            void Validation(ShoppingCartInfoItem shoppingCartInfoItem) 
                => shoppingCartInfoItem.Should().BeNull();

            await SalesModule.AssertEventuallyAsync
            (
                clockService,
                new PostgresDatabaseProbe<SalesDbContext, ShoppingCartInfoItem>(SalesModule, Sampling, Validation),
                TimeoutMillis
            );
        }
        
        [Theory, CustomAutoData]
        internal async Task Projecting_ShoppingCartDeliveryAddressSetDomainEvent_to_read_model
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount,
            Address deliveryAddress
        )
        {
            // Arrange
            IClockService clockService = new ClockService();
            
            ShoppingCart shoppingCart = ShoppingCart
                .Create(shoppingCartId, customerId, customerDiscount).Data;
            shoppingCart.Customer.SetDeliveryAddress(deliveryAddress);

            // Act
            await ShoppingCartHelper.SaveAsync(shoppingCart);
        
            // Assert
            Task<ShoppingCartInfo> Sampling(SalesDbContext dbContext) 
                => dbContext.ShoppingCarts
                    .FirstOrDefaultAsync(sc => sc.Id == shoppingCartId);

            void Validation(ShoppingCartInfo shoppingCartInfo)
            {
                shoppingCartInfo.Should().NotBeNull();
                shoppingCartInfo.Status.Should().Be(ShoppingCartStatus.AwaitingConfirmation);
            }

            await SalesModule.AssertEventuallyAsync
            (
                clockService,
                new PostgresDatabaseProbe<SalesDbContext, ShoppingCartInfo>(SalesModule, Sampling, Validation),
                TimeoutMillis
            );
        }
        
        [Theory, CustomAutoData]
        internal async Task Projecting_ShoppingCartCheckoutRequestedDomainEvent_to_read_model
        (
            ShoppingCart shoppingCart,
            EntityId orderId
        )
        {
            // Arrange
            IClockService clockService = new ClockService();

            shoppingCart.Checkout(orderId, clockService.Now);

            // Act
            await ShoppingCartHelper.SaveAsync(shoppingCart);
        
            // Assert
            Task<ShoppingCartInfo> Sampling(SalesDbContext dbContext) 
                => dbContext.ShoppingCarts
                    .FirstOrDefaultAsync(sc => sc.Id == shoppingCart.Id);

            void Validation(ShoppingCartInfo shoppingCartInfo)
            {
                shoppingCartInfo.Should().NotBeNull();
                shoppingCartInfo.Status.Should().Be(ShoppingCartStatus.PendingCheckout);
            }

            await SalesModule.AssertEventuallyAsync
            (
                clockService,
                new PostgresDatabaseProbe<SalesDbContext, ShoppingCartInfo>(SalesModule, Sampling, Validation),
                TimeoutMillis
            );
        }
        
        [Theory, CustomAutoData]
        internal async Task Projecting_ShoppingCartDeletionRequestedDomainEvent_to_read_model(ShoppingCart shoppingCart)
        {
            // Arrange
            IClockService clockService = new ClockService();

            shoppingCart.Delete();

            // Act
            await ShoppingCartHelper.SaveAsync(shoppingCart);
        
            // Assert
            Task<ShoppingCartInfo> Sampling(SalesDbContext dbContext) 
                => dbContext.ShoppingCarts
                    .FirstOrDefaultAsync(sc => sc.Id == shoppingCart.Id);

            void Validation(ShoppingCartInfo shoppingCartInfo)
            {
                shoppingCartInfo.Should().NotBeNull();
                shoppingCartInfo.Status.Should().Be(ShoppingCartStatus.Closed);
            }

            await SalesModule.AssertEventuallyAsync
            (
                clockService,
                new PostgresDatabaseProbe<SalesDbContext, ShoppingCartInfo>(SalesModule, Sampling, Validation),
                TimeoutMillis
            );
        }
        
        [Theory, CustomAutoData]
        internal async Task Projecting_ShoppingCartItemQuantityIncreasedDomainEvent_to_read_model
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
            
            ShoppingCart shoppingCart = ShoppingCart
                .Create(shoppingCartId, customerId, customerDiscount).Data;
            shoppingCart.AddProductQuantity(productId, ProductQuantity.Create(1).Data, productPrice);
            shoppingCart.AddProductQuantity(productId, ProductQuantity.Create(2).Data, productPrice);

            // Act
            await ShoppingCartHelper.SaveAsync(shoppingCart);
        
            // Assert
            Task<ShoppingCartInfoItem> Sampling(SalesDbContext dbContext) 
                => dbContext.ShoppingCartItems
                    .FirstOrDefaultAsync(sc => sc.ShoppingCartInfoId == shoppingCartId);

            void Validation(ShoppingCartInfoItem shoppingCartInfoItem)
            {
                shoppingCartInfoItem.Should().NotBeNull();
                shoppingCartInfoItem.Quantity.Should().Be(3);
            }

            await SalesModule.AssertEventuallyAsync
            (
                clockService,
                new PostgresDatabaseProbe<SalesDbContext, ShoppingCartInfoItem>(SalesModule, Sampling, Validation),
                TimeoutMillis
            );
        }
        
        [Theory, CustomAutoData]
        internal async Task Projecting_ShoppingCartItemQuantityDecreasedDomainEvent_to_read_model
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
            
            ShoppingCart shoppingCart = ShoppingCart
                .Create(shoppingCartId, customerId, customerDiscount).Data;
            shoppingCart.AddProductQuantity(productId, ProductQuantity.Create(10).Data, productPrice);
            shoppingCart.RemoveProductQuantity(productId, ProductQuantity.Create(2).Data);

            // Act
            await ShoppingCartHelper.SaveAsync(shoppingCart);
        
            // Assert
            Task<ShoppingCartInfoItem> Sampling(SalesDbContext dbContext) 
                => dbContext.ShoppingCartItems
                    .FirstOrDefaultAsync(sc => sc.ShoppingCartInfoId == shoppingCartId);

            void Validation(ShoppingCartInfoItem shoppingCartInfoItem)
            {
                shoppingCartInfoItem.Should().NotBeNull();
                shoppingCartInfoItem.Quantity.Should().Be(8);
            }

            await SalesModule.AssertEventuallyAsync
            (
                clockService,
                new PostgresDatabaseProbe<SalesDbContext, ShoppingCartInfoItem>(SalesModule, Sampling, Validation),
                TimeoutMillis
            );
        }
    }
}