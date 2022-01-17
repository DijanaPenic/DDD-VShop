using System;
using System.Threading.Tasks;

using VShop.Modules.Sales.Infrastructure.Entities;

namespace VShop.Modules.Sales.API.Application.Queries
{
    public interface IShoppingCartReadService
    {
        Task<ShoppingCartInfo> GetActiveShoppingCartByCustomerIdAsync(Guid customerId);
    }
}