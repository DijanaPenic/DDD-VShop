namespace VShop.Modules.Sales.Domain.ProcessManagers
{
    public class OrderFulfillmentProcessManager
    {
        public enum OrderFulfillmentStatus
        {
            CheckoutRequested = 1,
            OrderPlaced = 2,
            OrderBilled = 3,
            OrderCancelled = 4,
            OrderShipped = 5
        }
    }
}