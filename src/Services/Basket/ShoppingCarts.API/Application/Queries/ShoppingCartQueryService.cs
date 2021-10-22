using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.Services.ShoppingCarts.Infrastructure;
using VShop.Services.ShoppingCarts.Infrastructure.Entities;

using static VShop.Services.ShoppingCarts.Domain.Models.BasketAggregate.Basket;

namespace VShop.Services.ShoppingCarts.API.Application.Queries
{
    public class ShoppingCartQueryService : IShoppingCartQueryService 
    {
        private readonly BasketContext _dbContext;
        
        public ShoppingCartQueryService(BasketContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<BasketDetails> GetActiveBasketByCustomerIdAsync(Guid customerId)
        {
            return _dbContext.Baskets
                .SingleOrDefaultAsync(b => b.CustomerId == customerId && b.Status != BasketStatus.Closed);
        }
    }
}