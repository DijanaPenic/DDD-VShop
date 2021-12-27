namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    // TODO - review integration tests
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