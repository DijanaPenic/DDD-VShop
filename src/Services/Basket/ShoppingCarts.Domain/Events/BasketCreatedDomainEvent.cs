﻿using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.ShoppingCarts.Domain.Events
{
    public record BasketCreatedDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; init; }
        public Guid CustomerId { get; init; }
        public int CustomerDiscount { get; init; }
    }
}