using System;
using System.Threading.Tasks;

using VShop.Services.Sales.Infrastructure.Entities;

namespace VShop.Services.Sales.API.Application.Queries
{
    public interface IShoppingCartQueryService
    {
        Task<ShoppingCartInfo> GetActiveShoppingCartByCustomerIdAsync(Guid customerId);
    }
}