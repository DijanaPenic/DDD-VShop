using System;
using System.Threading.Tasks;

using VShop.Services.Basket.Infrastructure.Entities;

namespace VShop.Services.Basket.API.Application.Queries
{
    public interface IBasketQueryService
    {
        Task<BasketDetails> GetActiveBasketByCustomerIdAsync(Guid customerId);
    }
}