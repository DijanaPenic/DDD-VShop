﻿using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Billing.Integration.Events
{
    // Notification for Sales - need to update order status and then validate stock.
    public partial class PaymentFailedIntegrationEvent : IIntegrationEvent
    {
        public PaymentFailedIntegrationEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}