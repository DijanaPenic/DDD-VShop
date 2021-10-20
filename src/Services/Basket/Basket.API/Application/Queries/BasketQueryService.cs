﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.Services.Basket.Infrastructure;
using VShop.Services.Basket.Infrastructure.Entities;

using static VShop.Services.Basket.Domain.Models.BasketAggregate.Basket;

namespace VShop.Services.Basket.API.Application.Queries
{
    public class BasketQueryService : IBasketQueryService 
    {
        private readonly BasketContext _dbContext;
        
        public BasketQueryService(BasketContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<BasketDetails> GetActiveBasketByCustomerIdAsync(Guid customerId)
        {
            return _dbContext.Baskets
                .SingleOrDefaultAsync(b => b.CustomerId == customerId && b.Status != BasketStatus.Closed);
        }
    }
}