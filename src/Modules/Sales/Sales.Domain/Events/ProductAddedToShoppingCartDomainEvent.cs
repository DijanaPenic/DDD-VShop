﻿using System;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ProductAddedToShoppingCartDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public Guid ShoppingCartItemId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
    }
}