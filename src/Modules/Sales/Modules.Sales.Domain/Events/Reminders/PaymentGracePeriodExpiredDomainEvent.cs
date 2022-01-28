﻿using System;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Events.Reminders
{
    public partial class PaymentGracePeriodExpiredDomainEvent : IDomainEvent
    {
        public PaymentGracePeriodExpiredDomainEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}