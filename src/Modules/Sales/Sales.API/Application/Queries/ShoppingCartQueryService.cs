using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.Infrastructure.Entities;
using VShop.Modules.Sales.Domain.Models.ShoppingCartAggregate;

namespace VShop.Modules.Sales.API.Application.Queries
{
    public class ShoppingCartQueryService : IShoppingCartQueryService 
    {
        private readonly SalesContext _dbContext;
        
        public ShoppingCartQueryService(SalesContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ShoppingCartInfo> GetActiveShoppingCartByCustomerIdAsync(Guid customerId)
        {
            return _dbContext.Sales
                .Include(sc => sc.Items)
                .SingleOrDefaultAsync(sc => sc.CustomerId == customerId && sc.Status != ShoppingCart.ShoppingCartStatus.Closed);
        }
    }
}