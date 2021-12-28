namespace VShop.Modules.Sales.Domain.Enums
{
    public enum OrderStatus
    {
        Processing = 1,
        Paid = 2,
        PendingShipping = 3,
        Shipped = 4,
        Cancelled = 5
    }
}