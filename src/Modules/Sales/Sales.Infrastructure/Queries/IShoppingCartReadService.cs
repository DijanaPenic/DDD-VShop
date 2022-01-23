using System;
using System.Threading.Tasks;
using VShop.Modules.Sales.Infrastructure.DAL.Entities;

namespace VShop.Modules.Sales.Infrastructure.Queries
{
    public interface IShoppingCartReadService
    {
        Task<ShoppingCartInfo> GetActiveShoppingCartByCustomerIdAsync(Guid customerId);
    }
}