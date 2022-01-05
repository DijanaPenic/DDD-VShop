﻿namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    public enum OrderingProcessManagerStatus
    {
        CheckoutRequested = 1,
        OrderPlaced = 2,
        OrderPaymentSucceeded = 3,
        OrderPaymentFailed = 4,
        OrderStockConfirmed = 5,
        OrderCancelled = 6,
        OrderPendingShipping = 7,
        OrderShipped = 8
    }
}