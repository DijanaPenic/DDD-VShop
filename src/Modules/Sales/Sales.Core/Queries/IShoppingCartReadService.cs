using System;
using System.Threading.Tasks;
using VShop.Modules.Sales.Core.DAL.Entities;

namespace VShop.Modules.Sales.Core.Queries
{
    public interface IShoppingCartReadService
    {
        Task<ShoppingCartInfo> GetActiveShoppingCartByCustomerIdAsync(Guid customerId);
    }
}