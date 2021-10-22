using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.Services.ShoppingCarts.Domain.Events;
using VShop.Services.ShoppingCarts.Infrastructure;
using VShop.Services.ShoppingCarts.Infrastructure.Entities;
using VShop.SharedKernel.Infrastructure.Domain;

using static VShop.Services.ShoppingCarts.Domain.Models.BasketAggregate.Basket;

namespace VShop.Services.ShoppingCarts.API.Projections
{
    public static class BasketDetailsProjection
    {
        public static Func<Task> ProjectAsync(BasketContext dbContext, IDomainEvent eventData)
            => eventData switch
            {
                BasketCreatedDomainEvent e => () =>
                {
                    dbContext.Baskets.Add(new BasketDetails()
                    {
                        Id = e.BasketId,
                        Status = BasketStatus.New,
                        CustomerId = e.CustomerId
                    });
            
                    return Task.CompletedTask;
                },
                ProductAddedToBasketDomainEvent e => () =>
                {
                    dbContext.BasketItems.Add(new BasketDetailsProductItem()
                    {
                        Id = e.BasketItemId,
                        Quantity = e.Quantity,
                        ProductId = e.ProductId,
                        UnitPrice = e.UnitPrice,
                        BasketDetailsId = e.BasketId
                    });
            
                    return Task.CompletedTask;
                },
                ProductRemovedFromBasketDomainEvent e => async() =>
                {
                    BasketDetailsProductItem basketItem = await dbContext.BasketItems.SingleAsync(bi => 
                        bi.ProductId == e.ProductId && bi.BasketDetailsId == e.BasketId);
                    
                    dbContext.BasketItems.Remove(basketItem);
                },
                DeliveryAddressSetDomainEvent e => async() =>
                {
                    BasketDetails basket = await dbContext.Baskets.SingleAsync(b => b.Id == e.BasketId);
                    basket.Status = BasketStatus.Fulfilled;
            
                    dbContext.Baskets.Update(basket);
                },
                BasketCheckoutRequestedDomainEvent e => async() =>
                {
                    BasketDetails basket = await dbContext.Baskets.SingleAsync(b => b.Id == e.BasketId);
                    basket.Status = BasketStatus.PendingCheckout;
            
                    dbContext.Baskets.Update(basket);
                },
                BasketDeletionRequestedDomainEvent e => async() =>
                {
                    BasketDetails basket = await dbContext.Baskets.SingleAsync(b => b.Id == e.BasketId);
                    basket.Status = BasketStatus.Closed;
            
                    dbContext.Baskets.Update(basket);
                },
                BasketItemQuantityIncreasedDomainEvent e => async() =>
                {
                    BasketDetailsProductItem basketItem = await dbContext.BasketItems.SingleAsync(bi => 
                        bi.ProductId == e.ProductId && bi.BasketDetailsId == e.BasketId);
                    basketItem.Quantity += e.Quantity;
            
                    dbContext.BasketItems.Update(basketItem);
                },
                BasketItemQuantityDecreasedDomainEvent e => async() =>
                {
                    BasketDetailsProductItem basketItem = await dbContext.BasketItems.SingleAsync(bi => 
                        bi.ProductId == e.ProductId && bi.BasketDetailsId == e.BasketId);
                    basketItem.Quantity -= e.Quantity;
            
                    dbContext.BasketItems.Update(basketItem);
                },
                _ => null
            };
    }
}