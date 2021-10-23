using System;
using System.Threading.Tasks;

using VShop.Services.ShoppingCarts.Infrastructure.Entities;

namespace VShop.Services.ShoppingCarts.API.Application.Queries
{
    public interface IShoppingCartQueryService
    {
        Task<ShoppingCartInfo> GetActiveShoppingCartByCustomerIdAsync(Guid customerId);
    }
}