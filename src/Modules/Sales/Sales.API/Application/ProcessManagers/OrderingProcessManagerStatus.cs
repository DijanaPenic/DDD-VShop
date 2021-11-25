namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    public enum OrderingProcessManagerStatus
    {
        CheckoutRequested = 1,
        OrderPlaced = 2,
        OrderPaymentSucceeded = 3,
        OrderPaymentFailed = 4,
        OrderCancelled = 5,
        OrderShipped = 6
    }
}