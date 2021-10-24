using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Domain;
using VShop.Services.Sales.Domain.Events;
using VShop.Services.Sales.Domain.Models.ShoppingCartAggregate;
using VShop.Services.Sales.Infrastructure;
using VShop.Services.Sales.Infrastructure.Entities;

namespace VShop.Services.Sales.API.Projections
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
                        Status = ShoppingCart.ShoppingCartStatus.New,
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
                    ShoppingCartInfoItem shoppingCartItem = await dbContext.ShoppingCartItems.SingleAsync(sci => 
                        sci.ProductId == e.ProductId && sci.ShoppingCartInfoId == e.ShoppingCartId);
                    
                    dbContext.ShoppingCartItems.Remove(shoppingCartItem);
                },
                DeliveryAddressSetDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await dbContext.Sales.SingleAsync(sc => sc.Id == e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCart.ShoppingCartStatus.AwaitingConfirmation;
            
                    dbContext.Sales.Update(shoppingCart);
                },
                ShoppingCartCheckoutRequestedDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await dbContext.Sales.SingleAsync(sc => sc.Id == e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCart.ShoppingCartStatus.PendingCheckout;
            
                    dbContext.Sales.Update(shoppingCart);
                },
                ShoppingCartDeletionRequestedDomainEvent e => async() =>
                {
                    ShoppingCartInfo shoppingCart = await dbContext.Sales.SingleAsync(sc => sc.Id == e.ShoppingCartId);
                    shoppingCart.Status = ShoppingCart.ShoppingCartStatus.Closed;
            
                    dbContext.Sales.Update(shoppingCart);
                },
                ShoppingCartItemQuantityIncreasedDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await dbContext.ShoppingCartItems.SingleAsync(sci => 
                        sci.ProductId == e.ProductId && sci.ShoppingCartInfoId == e.ShoppingCartId);
                    shoppingCartItem.Quantity += e.Quantity;
            
                    dbContext.ShoppingCartItems.Update(shoppingCartItem);
                },
                ShoppingCartItemQuantityDecreasedDomainEvent e => async() =>
                {
                    ShoppingCartInfoItem shoppingCartItem = await dbContext.ShoppingCartItems.SingleAsync(sci => 
                        sci.ProductId == e.ProductId && sci.ShoppingCartInfoId == e.ShoppingCartId);
                    shoppingCartItem.Quantity -= e.Quantity;
            
                    dbContext.ShoppingCartItems.Update(shoppingCartItem);
                },
                _ => null
            };
    }
}