namespace VShop.Modules.Sales.Domain.Enums
{
    public enum OrderFulfillmentStatus
    {
        CheckoutRequested = 1,
        OrderPlaced = 2,
        OrderBilled = 3,
        OrderCancelled = 4,
        OrderShipped = 5,
        Terminated = 6
    }
}