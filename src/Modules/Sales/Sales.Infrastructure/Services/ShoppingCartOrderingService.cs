﻿using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.Infrastructure.Services
{
    public class ShoppingCartOrderingService : IShoppingCartOrderingService
    {
        private readonly IAggregateRepository<ShoppingCart> _shoppingCartRepository;

        public ShoppingCartOrderingService(IAggregateRepository<ShoppingCart> shoppingCartRepository)
            => _shoppingCartRepository = shoppingCartRepository;

        public async Task<Result<Order>> CreateOrderAsync
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Guid causationId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        )
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync
            (
                shoppingCartId,
                causationId,
                correlationId,
                cancellationToken
            );
            
            Order order = new()
            {
                CorrelationId = correlationId,
                CausationId = causationId,
            };
            
            Result createOrderResult = order.Create
            (
                orderId,
                shoppingCart.DeliveryCost,
                shoppingCart.TotalDiscount,
                shoppingCart.Customer.CustomerId,
                shoppingCart.Customer.FullName,
                shoppingCart.Customer.EmailAddress,
                shoppingCart.Customer.PhoneNumber,
                shoppingCart.Customer.DeliveryAddress
            );
            
            if (createOrderResult.IsError(out ApplicationError createOrderError)) return createOrderError;
            
            foreach (ShoppingCartItem item in shoppingCart.Items)
            {
                Result addOrderLineResult = order.AddOrderLine
                (
                    item.Id,
                    item.Quantity,
                    item.UnitPrice
                );

                if (addOrderLineResult.IsError(out ApplicationError addOrderLineError)) return addOrderLineError;
            }

            return order;
        }
    }
}