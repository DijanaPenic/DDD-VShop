using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.Services.Basket.Domain.Events;
using VShop.Services.Basket.Infrastructure;
using VShop.Services.Basket.Infrastructure.Entities;

using static VShop.Services.Basket.Domain.Models.BasketAggregate.Basket;

namespace VShop.Services.Basket.API.Projections
{
    public static class BasketDetailsProjection
    {
        public static Func<Task> ProjectAsync(BasketContext dbContext, object @event)
            => @event switch
            {
                BasketCreatedDomainEvent e => () => HandleAsync(dbContext, e),
                ProductAddedToBasketDomainEvent e => () => HandleAsync(dbContext, e),
                ProductRemovedFromBasketDomainEvent e => () => HandleAsync(dbContext, e),
                DeliveryAddressSetDomainEvent e => () => HandleAsync(dbContext, e),
                BasketCheckoutRequestedDomainEvent e => () => HandleAsync(dbContext, e),
                BasketDeletionRequestedDomainEvent e => () => HandleAsync(dbContext, e),
                BasketItemQuantityIncreasedDomainEvent e => () => HandleAsync(dbContext, e),
                BasketItemQuantityDecreasedDomainEvent e => () => HandleAsync(dbContext, e),
                _ => null
            };

        private static Task HandleAsync(BasketContext dbContext, BasketCreatedDomainEvent @event)
        {
            dbContext.Baskets.Add(new BasketDetails()
            {
                Id = @event.BasketId,
                Status = BasketStatus.New,
                CustomerId = @event.CustomerId
            });
            
            return Task.CompletedTask;
        }
        
        private static Task HandleAsync(BasketContext dbContext, ProductAddedToBasketDomainEvent @event)
        {
            dbContext.BasketItems.Add(new BasketDetailsProductItem()
            {
                Id = @event.BasketItemId,
                Quantity = @event.Quantity,
                ProductId = @event.ProductId,
                UnitPrice = @event.UnitPrice,
                BasketDetailsId = @event.BasketId
            });
            
            return Task.CompletedTask;
        }
        
        private static async Task HandleAsync(BasketContext dbContext, ProductRemovedFromBasketDomainEvent @event)
        {
            BasketDetailsProductItem basketItem = await dbContext.BasketItems.SingleAsync(bi => 
                bi.ProductId == @event.ProductId && bi.BasketDetailsId == @event.BasketId);
            
            dbContext.BasketItems.Remove(basketItem);
        }
        
        private static async Task HandleAsync(BasketContext dbContext, DeliveryAddressSetDomainEvent @event)
        {
            BasketDetails basket = await dbContext.Baskets.SingleAsync(b => b.Id == @event.BasketId);
            basket.Status = BasketStatus.Fulfilled;
            
            dbContext.Baskets.Update(basket);
        }
        
        private static async Task HandleAsync(BasketContext dbContext, BasketCheckoutRequestedDomainEvent @event)
        {
            BasketDetails basket = await dbContext.Baskets.SingleAsync(b => b.Id == @event.BasketId);
            basket.Status = BasketStatus.PendingCheckout;
            
            dbContext.Baskets.Update(basket);
        }
        
        private static async Task HandleAsync(BasketContext dbContext, BasketDeletionRequestedDomainEvent @event)
        {
            BasketDetails basket = await dbContext.Baskets.SingleAsync(b => b.Id == @event.BasketId);
            basket.Status = BasketStatus.Closed;
            
            dbContext.Baskets.Update(basket);
        }
        
        private static async Task HandleAsync(BasketContext dbContext, BasketItemQuantityIncreasedDomainEvent @event)
        {                    
            BasketDetailsProductItem basketItem = await dbContext.BasketItems.SingleAsync(bi => 
                bi.ProductId == @event.ProductId && bi.BasketDetailsId == @event.BasketId);
            basketItem.Quantity += @event.Quantity;
            
            dbContext.BasketItems.Update(basketItem);
        }
        
        private static async Task HandleAsync(BasketContext dbContext, BasketItemQuantityDecreasedDomainEvent @event)
        {
            BasketDetailsProductItem basketItem = await dbContext.BasketItems.SingleAsync(bi => 
                bi.ProductId == @event.ProductId && bi.BasketDetailsId == @event.BasketId);
            basketItem.Quantity -= @event.Quantity;
            
            dbContext.BasketItems.Update(basketItem);
        }
    }
}