using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Infrastructure.Domain;
using VShop.Services.ShoppingCarts.Domain.Events;
using VShop.Services.ShoppingCarts.Infrastructure;
using VShop.Services.ShoppingCarts.Infrastructure.Entities;

using static VShop.Services.ShoppingCarts.Domain.Models.ShoppingCartAggregate.ShoppingCart;

namespace VShop.Services.ShoppingCarts.API.Projections
{
    public static class ShoppingCartDetailsProjection
    {
        public static Func<Task> ProjectAsync(ShoppingCartContext dbContext, IDomainEvent eventData)
            => eventData switch
            {
                ShoppingCartCreatedDomainEvent e => () =>
                {
                    dbContext.ShoppingCarts.Add(new ShoppingCartInfo()
                    {
                        Id = e.ShoppingCartId,
                        Status = ShoppingCartStatus.New,
                        CustomerId = e.CustomerId
                    });
            
                    return Task.CompletedTask;
                },
                ProductAddedToShoppingCartDomainEvent e => () =>
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
                ProductRemovedFromShoppingCartDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await dbContext.ShoppingCartItems.SingleAsync(bi => 
                        bi.ProductId == e.ProductId && bi.ShoppingCartInfoId == e.ShoppingCartId);
                    
                    dbContext.ShoppingCartItems.Remove(shoppingCartItem);
                },
                DeliveryAddressSetDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await dbContext.ShoppingCarts.SingleAsync(b => b.Id == e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCartStatus.Fulfilled;
            
                    dbContext.ShoppingCarts.Update(shoppingCart);
                },
                ShoppingCartCheckoutRequestedDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await dbContext.ShoppingCarts.SingleAsync(b => b.Id == e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCartStatus.PendingCheckout;
            
                    dbContext.ShoppingCarts.Update(shoppingCart);
                },
                ShoppingCartDeletionRequestedDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await dbContext.ShoppingCarts.SingleAsync(b => b.Id == e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCartStatus.Closed;
            
                    dbContext.ShoppingCarts.Update(shoppingCart);
                },
                ShoppingCartItemQuantityIncreasedDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await dbContext.ShoppingCartItems.SingleAsync(bi => 
                        bi.ProductId == e.ProductId && bi.ShoppingCartInfoId == e.ShoppingCartId);
                    shoppingCartItem.Quantity += e.Quantity;
            
                    dbContext.ShoppingCartItems.Update(shoppingCartItem);
                },
                ShoppingCartItemQuantityDecreasedDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await dbContext.ShoppingCartItems.SingleAsync(bi => 
                        bi.ProductId == e.ProductId && bi.ShoppingCartInfoId == e.ShoppingCartId);
                    shoppingCartItem.Quantity -= e.Quantity;
            
                    dbContext.ShoppingCartItems.Update(shoppingCartItem);
                },
                _ => null
            };
    }
}