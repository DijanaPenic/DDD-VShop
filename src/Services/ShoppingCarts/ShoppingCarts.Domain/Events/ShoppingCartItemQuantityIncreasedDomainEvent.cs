﻿using System;

using VShop.SharedKernel.Domain;

namespace VShop.Services.ShoppingCarts.Domain.Events
{
    public record ShoppingCartItemQuantityIncreasedDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}