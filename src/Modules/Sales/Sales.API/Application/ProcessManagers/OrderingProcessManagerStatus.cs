namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    public enum OrderingProcessManagerStatus
    {
        CheckoutRequested = 1,
        OrderPlaced = 2,
        OrderBilled = 3,
        OrderCancelled = 4,
        OrderShipped = 5,
        Terminated = 6
    }
}