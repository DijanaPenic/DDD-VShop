namespace VShop.Modules.Sales.Domain.Enums
{
    internal enum ShoppingCartStatus
    {
        New = 1,
        AwaitingConfirmation = 2,     // Customer has provided needed contact information and is allowed to proceed with checkout.
        PendingCheckout = 3,          // Checkout has been requested. The next step: payment.
        Closed = 4                    // Shopping cart has been deleted (soft delete).
    }
}