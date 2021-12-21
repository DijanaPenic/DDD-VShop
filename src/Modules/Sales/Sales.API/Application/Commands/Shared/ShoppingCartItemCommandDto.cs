using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.Modules.Sales.API.Application.Commands.Shared
{
    public record ShoppingCartItemCommandDto
    {
        public EntityId ProductId { get; init; }
        public Price UnitPrice { get; init; }
        public ProductQuantity Quantity { get; init; }
    }
}