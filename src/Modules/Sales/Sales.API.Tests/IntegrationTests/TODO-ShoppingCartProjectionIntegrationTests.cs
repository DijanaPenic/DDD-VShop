// using System;
// using Xunit;
// using FluentAssertions;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
//
// using VShop.Modules.Sales.Domain.Enums;
// using VShop.Modules.Sales.Infrastructure;
// using VShop.Modules.Sales.Infrastructure.Entities;
// using VShop.Modules.Sales.Tests.Customizations;
// using VShop.Modules.Sales.Domain.Models.ShoppingCart;
// using VShop.Modules.Sales.API.Tests.IntegrationTests.Helpers;
// using VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure;
// using VShop.SharedKernel.Domain.ValueObjects;
// using VShop.SharedKernel.Infrastructure.Services;
// using VShop.SharedKernel.Infrastructure.Services.Contracts;
//
// namespace VShop.Modules.Sales.API.Tests.IntegrationTests
// {
//     [Collection("Non-Parallel Tests Collection")]
//     public class ShoppingCartProjectionIntegrationTests : IClassFixture<SubscriptionFixture>
//     {
//         private const int TimeoutMillis = 3000;
//             
//         [Theory]
//         [CustomizedAutoData]
//         public async Task Projecting_ShoppingCartCreatedDomainEvent_to_read_model
//         (
//             EntityId shoppingCartId,
//             EntityId customerId,
//             Discount customerDiscount,
//             Guid causationId,
//             Guid correlationId
//         )
//         {
//             // Arrange
//             IClockService clockService = new ClockService();
//
//             ShoppingCart shoppingCart = ShoppingCart.Create
//                 (shoppingCartId, customerId, customerDiscount, causationId, correlationId).Data;
//             
//             // Act
//             await ShoppingCartHelper.SaveAsync(shoppingCart);
//         
//             // Assert
//             Task<ShoppingCartInfo> Sampling(SalesContext dbContext) 
//                 => dbContext.ShoppingCarts.FirstOrDefaultAsync(sc => sc.Id == shoppingCartId);
//
//             void Validation(ShoppingCartInfo shoppingCartInfo)
//             { 
//                 shoppingCartInfo.Should().NotBeNull();
//                 shoppingCartInfo!.Status.Should().Be(ShoppingCartStatus.New);
//                 shoppingCartInfo.CustomerId.Should().Be(customerId);
//             }
//
//             await IntegrationTestsFixture.AssertEventuallyAsync
//             (
//                 clockService,
//                 new DatabaseProbe<SalesContext, ShoppingCartInfo>(Sampling, Validation),
//                 TimeoutMillis
//             );
//         }
//         
//         [Theory]
//         [CustomizedAutoData]
//         public async Task Projecting_ShoppingCartProductAddedDomainEvent_to_read_model
//         (
//             EntityId shoppingCartId,
//             EntityId customerId,
//             Discount customerDiscount,
//             EntityId productId,
//             ProductQuantity productQuantity,
//             Price productPrice,
//             Guid causationId,
//             Guid correlationId
//         )
//         {
//             // Arrange
//             IClockService clockService = new ClockService();
//             
//             ShoppingCart shoppingCart = ShoppingCart
//                 .Create(shoppingCartId, customerId, customerDiscount, causationId, correlationId).Data;
//             shoppingCart.AddProduct(productId, productQuantity, productPrice);
//
//             // Act
//             await ShoppingCartHelper.SaveAsync(shoppingCart);
//         
//             // Assert
//             Task<ShoppingCartInfoItem> Sampling(SalesContext dbContext) 
//                 => dbContext.ShoppingCartItems
//                     .FirstOrDefaultAsync(sc => sc.ShoppingCartInfoId == shoppingCartId);
//
//             void Validation(ShoppingCartInfoItem shoppingCartInfoItem)
//             { 
//                 shoppingCartInfoItem.Should().NotBeNull();
//                 shoppingCartInfoItem!.Quantity.Should().Be(productQuantity);
//                 shoppingCartInfoItem.ProductId.Should().Be(productId);
//                 shoppingCartInfoItem.UnitPrice.Should().Be(productPrice);
//             }
//
//             await IntegrationTestsFixture.AssertEventuallyAsync
//             (
//                 clockService,
//                 new DatabaseProbe<SalesContext, ShoppingCartInfoItem>(Sampling, Validation),
//                 TimeoutMillis
//             );
//         }
//         
//         [Theory]
//         [CustomizedAutoData]
//         public async Task Projecting_ShoppingCartProductRemovedDomainEvent_to_read_model
//         (
//             EntityId shoppingCartId,
//             EntityId customerId,
//             Discount customerDiscount,
//             EntityId productId,
//             ProductQuantity productQuantity,
//             Price productPrice,
//             Guid causationId,
//             Guid correlationId
//         )
//         {
//             // Arrange
//             IClockService clockService = new ClockService();
//             
//             ShoppingCart shoppingCart = ShoppingCart
//                 .Create(shoppingCartId, customerId, customerDiscount, causationId, correlationId).Data;
//             shoppingCart.AddProduct(productId, productQuantity, productPrice);
//             shoppingCart.RemoveProduct(productId, productQuantity);
//
//             // Act
//             await ShoppingCartHelper.SaveAsync(shoppingCart);
//         
//             // Assert
//             Task<ShoppingCartInfoItem> Sampling(SalesContext dbContext) 
//                 => dbContext.ShoppingCartItems
//                     .FirstOrDefaultAsync(sc => sc.ShoppingCartInfoId == shoppingCartId);
//
//             void Validation(ShoppingCartInfoItem shoppingCartInfoItem) 
//                 => shoppingCartInfoItem.Should().BeNull();
//
//             await IntegrationTestsFixture.AssertEventuallyAsync
//             (
//                 clockService,
//                 new DatabaseProbe<SalesContext, ShoppingCartInfoItem>(Sampling, Validation),
//                 TimeoutMillis
//             );
//         }
//         
//         [Theory]
//         [CustomizedAutoData]
//         public async Task Projecting_ShoppingCartDeliveryAddressSetDomainEvent_to_read_model
//         (
//             EntityId shoppingCartId,
//             EntityId customerId,
//             Discount customerDiscount,
//             Address deliveryAddress,
//             Guid causationId,
//             Guid correlationId
//         )
//         {
//             // Arrange
//             IClockService clockService = new ClockService();
//             
//             ShoppingCart shoppingCart = ShoppingCart
//                 .Create(shoppingCartId, customerId, customerDiscount, causationId, correlationId).Data;
//             shoppingCart.Customer.SetDeliveryAddress(deliveryAddress);
//
//             // Act
//             await ShoppingCartHelper.SaveAsync(shoppingCart);
//         
//             // Assert
//             Task<ShoppingCartInfo> Sampling(SalesContext dbContext) 
//                 => dbContext.ShoppingCarts
//                     .FirstOrDefaultAsync(sc => sc.Id == shoppingCartId);
//
//             void Validation(ShoppingCartInfo shoppingCartInfo)
//             {
//                 shoppingCartInfo.Should().NotBeNull();
//                 shoppingCartInfo.Status.Should().Be(ShoppingCartStatus.AwaitingConfirmation);
//             }
//
//             await IntegrationTestsFixture.AssertEventuallyAsync
//             (
//                 clockService,
//                 new DatabaseProbe<SalesContext, ShoppingCartInfo>(Sampling, Validation),
//                 TimeoutMillis
//             );
//         }
//         
//         [Theory]
//         [CustomizedAutoData]
//         public async Task Projecting_ShoppingCartCheckoutRequestedDomainEvent_to_read_model
//         (
//             ShoppingCart shoppingCart,
//             EntityId orderId
//         )
//         {
//             // Arrange
//             IClockService clockService = new ClockService();
//
//             shoppingCart.RequestCheckout(orderId, clockService.Now);
//
//             // Act
//             await ShoppingCartHelper.SaveAsync(shoppingCart);
//         
//             // Assert
//             Task<ShoppingCartInfo> Sampling(SalesContext dbContext) 
//                 => dbContext.ShoppingCarts
//                     .FirstOrDefaultAsync(sc => sc.Id == shoppingCart.Id);
//
//             void Validation(ShoppingCartInfo shoppingCartInfo)
//             {
//                 shoppingCartInfo.Should().NotBeNull();
//                 shoppingCartInfo.Status.Should().Be(ShoppingCartStatus.PendingCheckout);
//             }
//
//             await IntegrationTestsFixture.AssertEventuallyAsync
//             (
//                 clockService,
//                 new DatabaseProbe<SalesContext, ShoppingCartInfo>(Sampling, Validation),
//                 TimeoutMillis
//             );
//         }
//         
//         [Theory]
//         [CustomizedAutoData]
//         public async Task Projecting_ShoppingCartDeletionRequestedDomainEvent_to_read_model(ShoppingCart shoppingCart)
//         {
//             // Arrange
//             IClockService clockService = new ClockService();
//
//             shoppingCart.RequestDelete();
//
//             // Act
//             await ShoppingCartHelper.SaveAsync(shoppingCart);
//         
//             // Assert
//             Task<ShoppingCartInfo> Sampling(SalesContext dbContext) 
//                 => dbContext.ShoppingCarts
//                     .FirstOrDefaultAsync(sc => sc.Id == shoppingCart.Id);
//
//             void Validation(ShoppingCartInfo shoppingCartInfo)
//             {
//                 shoppingCartInfo.Should().NotBeNull();
//                 shoppingCartInfo.Status.Should().Be(ShoppingCartStatus.Closed);
//             }
//
//             await IntegrationTestsFixture.AssertEventuallyAsync
//             (
//                 clockService,
//                 new DatabaseProbe<SalesContext, ShoppingCartInfo>(Sampling, Validation),
//                 TimeoutMillis
//             );
//         }
//         
//         [Theory]
//         [CustomizedAutoData]
//         public async Task Projecting_ShoppingCartItemQuantityIncreasedDomainEvent_to_read_model
//         (
//             EntityId shoppingCartId,
//             EntityId customerId,
//             Discount customerDiscount,
//             EntityId productId,
//             Price productPrice,
//             Guid causationId,
//             Guid correlationId
//         )
//         {
//             // Arrange
//             IClockService clockService = new ClockService();
//             
//             ShoppingCart shoppingCart = ShoppingCart
//                 .Create(shoppingCartId, customerId, customerDiscount, causationId, correlationId).Data;
//             shoppingCart.AddProduct(productId, ProductQuantity.Create(1).Data, productPrice);
//             shoppingCart.AddProduct(productId, ProductQuantity.Create(2).Data, productPrice);
//
//             // Act
//             await ShoppingCartHelper.SaveAsync(shoppingCart);
//         
//             // Assert
//             Task<ShoppingCartInfoItem> Sampling(SalesContext dbContext) 
//                 => dbContext.ShoppingCartItems
//                     .FirstOrDefaultAsync(sc => sc.ShoppingCartInfoId == shoppingCartId);
//
//             void Validation(ShoppingCartInfoItem shoppingCartInfoItem)
//             {
//                 shoppingCartInfoItem.Should().NotBeNull();
//                 shoppingCartInfoItem.Quantity.Should().Be(3);
//             }
//
//             await IntegrationTestsFixture.AssertEventuallyAsync
//             (
//                 clockService,
//                 new DatabaseProbe<SalesContext, ShoppingCartInfoItem>(Sampling, Validation),
//                 TimeoutMillis
//             );
//         }
//         
//         [Theory]
//         [CustomizedAutoData]
//         public async Task Projecting_ShoppingCartItemQuantityDecreasedDomainEvent_to_read_model
//         (
//             EntityId shoppingCartId,
//             EntityId customerId,
//             Discount customerDiscount,
//             EntityId productId,
//             Price productPrice,
//             Guid causationId,
//             Guid correlationId
//         )
//         {
//             // Arrange
//             IClockService clockService = new ClockService();
//             
//             ShoppingCart shoppingCart = ShoppingCart
//                 .Create(shoppingCartId, customerId, customerDiscount, causationId, correlationId).Data;
//             shoppingCart.AddProduct(productId, ProductQuantity.Create(10).Data, productPrice);
//             shoppingCart.RemoveProduct(productId, ProductQuantity.Create(2).Data);
//
//             // Act
//             await ShoppingCartHelper.SaveAsync(shoppingCart);
//         
//             // Assert
//             Task<ShoppingCartInfoItem> Sampling(SalesContext dbContext) 
//                 => dbContext.ShoppingCartItems
//                     .FirstOrDefaultAsync(sc => sc.ShoppingCartInfoId == shoppingCartId);
//
//             void Validation(ShoppingCartInfoItem shoppingCartInfoItem)
//             {
//                 shoppingCartInfoItem.Should().NotBeNull();
//                 shoppingCartInfoItem.Quantity.Should().Be(8);
//             }
//
//             await IntegrationTestsFixture.AssertEventuallyAsync
//             (
//                 clockService,
//                 new DatabaseProbe<SalesContext, ShoppingCartInfoItem>(Sampling, Validation),
//                 TimeoutMillis
//             );
//         }
//     }
// }