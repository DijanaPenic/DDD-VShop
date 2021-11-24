using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.Infrastructure.Entities;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.API.Projections
{
    public static class ShoppingCartInfoProjection
    {
        public static Func<Task> ProjectAsync(SalesContext salesContext, IDomainEvent @event)
            => @event switch
            {
                ShoppingCartCreatedDomainEvent e => () =>
                {
                    salesContext.ShoppingCarts.Add(new ShoppingCartInfo()
                    {
                        Id = e.ShoppingCartId,
                        Status = ShoppingCartStatus.New,
                        CustomerId = e.CustomerId
                    });
            
                    return Task.CompletedTask;
                },
                ShoppingCartProductAddedDomainEvent e => () =>
                {
                    salesContext.ShoppingCartItems.Add(new ShoppingCartInfoItem()
                    {
                        Quantity = e.Quantity,
                        ProductId = e.ProductId,
                        UnitPrice = e.UnitPrice,
                        ShoppingCartInfoId = e.ShoppingCartId
                    });
            
                    return Task.CompletedTask;
                },
                ShoppingCartProductRemovedDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await GetShoppingCartItemAsync(salesContext, e.ProductId, e.ShoppingCartId);
                    
                    salesContext.ShoppingCartItems.Remove(shoppingCartItem);
                },
                ShoppingCartDeliveryAddressSetDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await GetShoppingCartAsync(salesContext, e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCartStatus.AwaitingConfirmation;
            
                    salesContext.ShoppingCarts.Update(shoppingCart);
                },
                ShoppingCartCheckoutRequestedDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await GetShoppingCartAsync(salesContext, e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCartStatus.PendingCheckout;
            
                    salesContext.ShoppingCarts.Update(shoppingCart);
                },
                ShoppingCartDeletionRequestedDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await GetShoppingCartAsync(salesContext, e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCartStatus.Closed;
            
                    salesContext.ShoppingCarts.Update(shoppingCart);
                },
                ShoppingCartItemQuantityIncreasedDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await GetShoppingCartItemAsync(salesContext, e.ProductId, e.ShoppingCartId);
                    shoppingCartItem.Quantity += e.Quantity;
            
                    salesContext.ShoppingCartItems.Update(shoppingCartItem);
                },
                ShoppingCartItemQuantityDecreasedDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await GetShoppingCartItemAsync(salesContext, e.ProductId, e.ShoppingCartId);
                    shoppingCartItem.Quantity -= e.Quantity;
            
                    salesContext.ShoppingCartItems.Update(shoppingCartItem);
                },
                _ => null
            };

        private static Task<ShoppingCartInfoItem> GetShoppingCartItemAsync(SalesContext salesContext, Guid productId, Guid shoppingCartId)
            => salesContext.ShoppingCartItems.SingleAsync(sci => 
                sci.ProductId == productId && sci.ShoppingCartInfoId == shoppingCartId);

        private static Task<ShoppingCartInfo> GetShoppingCartAsync(SalesContext salesContext, Guid shoppingCartId)
            => salesContext.ShoppingCarts.SingleAsync(sc => sc.Id == shoppingCartId);
    }
}