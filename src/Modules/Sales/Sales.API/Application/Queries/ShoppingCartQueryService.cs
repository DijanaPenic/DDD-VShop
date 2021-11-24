﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.Infrastructure.Entities;

namespace VShop.Modules.Sales.API.Application.Queries
{
    public class ShoppingCartQueryService : IShoppingCartQueryService 
    {
        private readonly SalesContext _salesContext;
        
        public ShoppingCartQueryService(SalesContext salesContext) => _salesContext = salesContext;

        public Task<ShoppingCartInfo> GetActiveShoppingCartByCustomerIdAsync(Guid customerId)
        {
            return _salesContext.ShoppingCarts
                .Include(sc => sc.Items)
                .SingleOrDefaultAsync(sc => sc.CustomerId == customerId && sc.Status != ShoppingCartStatus.Closed);
        }
    }
}