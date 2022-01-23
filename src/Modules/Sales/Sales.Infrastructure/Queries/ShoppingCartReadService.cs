using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Sales.Infrastructure.DAL;
using VShop.Modules.Sales.Infrastructure.DAL.Entities;
using VShop.Modules.Sales.Domain.Enums;

namespace VShop.Modules.Sales.Infrastructure.Queries
{
    public class ShoppingCartReadService : IShoppingCartReadService 
    {
        private readonly SalesDbContext _salesDbContext;
        
        public ShoppingCartReadService(SalesDbContext salesDbContext) => _salesDbContext = salesDbContext;

        public Task<ShoppingCartInfo> GetActiveShoppingCartByCustomerIdAsync(Guid customerId)
        {
            return _salesDbContext.ShoppingCarts
                .Include(sc => sc.Items)
                .SingleOrDefaultAsync(sc => sc.CustomerId == customerId && sc.Status != ShoppingCartStatus.Closed);
        }
    }
}