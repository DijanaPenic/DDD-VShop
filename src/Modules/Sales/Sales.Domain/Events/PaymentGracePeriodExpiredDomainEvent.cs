﻿using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record PaymentGracePeriodExpiredDomainEvent : DomainEvent
    {
        public Guid OrderId { get; }

        public PaymentGracePeriodExpiredDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}