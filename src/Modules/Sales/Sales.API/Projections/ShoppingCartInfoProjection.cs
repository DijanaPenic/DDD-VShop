using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.Infrastructure.Entities;
using VShop.SharedKernel.Infrastructure.Events;

namespace VShop.Modules.Sales.API.Projections
{
    public static class ShoppingCartInfoProjection
    {
        public static Func<Task> ProjectAsync(SalesDbContext salesDbContext, IDomainEvent @event)
            => @event switch
            {
                ShoppingCartCreatedDomainEvent e => () =>
                {
                    salesDbContext.ShoppingCarts.Add(new ShoppingCartInfo()
                    {
                        Id = e.ShoppingCartId,
                        Status = ShoppingCartStatus.New,
                        CustomerId = e.CustomerId
                    });
            
                    return Task.CompletedTask;
                },
                ShoppingCartProductAddedDomainEvent e => () =>
                {
                    salesDbContext.ShoppingCartItems.Add(new ShoppingCartInfoItem()
                    {
                        Quantity = e.Quantity,
                        ProductId = e.ProductId,
                        UnitPrice = e.UnitPrice.DecimalValue,
                        ShoppingCartInfoId = e.ShoppingCartId
                    });
            
                    return Task.CompletedTask;
                },
                ShoppingCartProductRemovedDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await GetShoppingCartItemAsync(salesDbContext, e.ProductId, e.ShoppingCartId);
                    
                    salesDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                },
                ShoppingCartDeliveryAddressSetDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await GetShoppingCartAsync(salesDbContext, e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCartStatus.AwaitingConfirmation;
            
                    salesDbContext.ShoppingCarts.Update(shoppingCart);
                },
                ShoppingCartCheckoutRequestedDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await GetShoppingCartAsync(salesDbContext, e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCartStatus.PendingCheckout;
            
                    salesDbContext.ShoppingCarts.Update(shoppingCart);
                },
                ShoppingCartDeletedDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await GetShoppingCartAsync(salesDbContext, e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCartStatus.Closed;
            
                    salesDbContext.ShoppingCarts.Update(shoppingCart);
                },
                ShoppingCartProductQuantityIncreasedDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await GetShoppingCartItemAsync(salesDbContext, e.ProductId, e.ShoppingCartId);
                    shoppingCartItem.Quantity += e.Quantity;
            
                    salesDbContext.ShoppingCartItems.Update(shoppingCartItem);
                },
                ShoppingCartProductQuantityDecreasedDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await GetShoppingCartItemAsync(salesDbContext, e.ProductId, e.ShoppingCartId);
                    shoppingCartItem.Quantity -= e.Quantity;
            
                    salesDbContext.ShoppingCartItems.Update(shoppingCartItem);
                },
                _ => null
            };

        private static Task<ShoppingCartInfoItem> GetShoppingCartItemAsync(SalesDbContext salesDbContext, Guid productId, Guid shoppingCartId)
            => salesDbContext.ShoppingCartItems.SingleAsync(sci => 
                sci.ProductId == productId && sci.ShoppingCartInfoId == shoppingCartId);

        private static Task<ShoppingCartInfo> GetShoppingCartAsync(SalesDbContext salesDbContext, Guid shoppingCartId)
            => salesDbContext.ShoppingCarts.SingleAsync(sc => sc.Id == shoppingCartId);
    }
}