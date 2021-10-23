using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.Services.ShoppingCarts.Infrastructure;
using VShop.Services.ShoppingCarts.Infrastructure.Entities;

using static VShop.Services.ShoppingCarts.Domain.Models.ShoppingCartAggregate.ShoppingCart;

namespace VShop.Services.ShoppingCarts.API.Application.Queries
{
    public class ShoppingCartQueryService : IShoppingCartQueryService 
    {
        private readonly ShoppingCartContext _dbContext;
        
        public ShoppingCartQueryService(ShoppingCartContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ShoppingCart> GetActiveShoppingCartByCustomerIdAsync(Guid customerId)
        {
            return _dbContext.ShoppingCarts
                .SingleOrDefaultAsync(b => b.CustomerId == customerId && b.Status != ShoppingCartStatus.Closed);
        }
    }
}