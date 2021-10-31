using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.Infrastructure.Entities;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.API.Projections
{
    public static class ShoppingCartInfoProjection
    {
        public static Func<Task> ProjectAsync(SalesContext dbContext, IDomainEvent eventData)
            => eventData switch
            {
                ShoppingCartCreatedDomainEvent e => () =>
                {
                    dbContext.Sales.Add(new ShoppingCartInfo()
                    {
                        Id = e.ShoppingCartId,
                        Status = ShoppingCartStatus.New,
                        CustomerId = e.CustomerId
                    });
            
                    return Task.CompletedTask;
                },
                ShoppingCartProductAddedDomainEvent e => () =>
                {
                    dbContext.ShoppingCartItems.Add(new ShoppingCartInfoItem()
                    {
                        Id = e.ShoppingCartItemId,
                        Quantity = e.Quantity,
                        ProductId = e.ProductId,
                        UnitPrice = e.UnitPrice,
                        ShoppingCartInfoId = e.ShoppingCartId
                    });
            
                    return Task.CompletedTask;
                },
                ShoppingCartProductRemovedDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await GetShoppingCartItemAsync(dbContext, e.ProductId, e.ShoppingCartId);
                    
                    dbContext.ShoppingCartItems.Remove(shoppingCartItem);
                },
                ShoppingCartDeliveryAddressSetDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await GetShoppingCartAsync(dbContext, e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCartStatus.AwaitingConfirmation;
            
                    dbContext.Sales.Update(shoppingCart);
                },
                ShoppingCartCheckoutRequestedDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await GetShoppingCartAsync(dbContext, e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCartStatus.PendingCheckout;
            
                    dbContext.Sales.Update(shoppingCart);
                },
                ShoppingCartDeletionRequestedDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await GetShoppingCartAsync(dbContext, e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCartStatus.Closed;
            
                    dbContext.Sales.Update(shoppingCart);
                },
                ShoppingCartItemQuantityIncreasedDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await GetShoppingCartItemAsync(dbContext, e.ProductId, e.ShoppingCartId);
                    shoppingCartItem.Quantity += e.Quantity;
            
                    dbContext.ShoppingCartItems.Update(shoppingCartItem);
                },
                ShoppingCartItemQuantityDecreasedDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await GetShoppingCartItemAsync(dbContext, e.ProductId, e.ShoppingCartId);
                    shoppingCartItem.Quantity -= e.Quantity;
            
                    dbContext.ShoppingCartItems.Update(shoppingCartItem);
                },
                _ => null
            };

        private static Task<ShoppingCartInfoItem> GetShoppingCartItemAsync(SalesContext dbContext, Guid productId, Guid shoppingCartId)
            => dbContext.ShoppingCartItems.SingleAsync(sci => 
                sci.ProductId == productId && sci.ShoppingCartInfoId == shoppingCartId);

        private static Task<ShoppingCartInfo> GetShoppingCartAsync(SalesContext dbContext, Guid shoppingCartId)
            => dbContext.Sales.SingleAsync(sc => sc.Id == shoppingCartId);
    }
}