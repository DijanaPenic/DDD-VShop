﻿using System;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Domain.Events
{
    public record OrderStatusSetToCancelledDomainEvent : BaseDomainEvent
    {
        public Guid OrderId { get; init; }
    }
}