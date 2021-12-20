namespace VShop.Modules.Sales.API.Models
{
    public record RemoveShoppingCartProductRequest
    {
        public int Quantity { get; init; }
    }
}