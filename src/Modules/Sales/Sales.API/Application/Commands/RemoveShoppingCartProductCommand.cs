using System;
using OneOf.Types;

using VShop.SharedKernel.Application.Commands;

namespace VShop.Services.Sales.API.Application.Commands
{
    public record RemoveShoppingCartProductCommand : ICommand<Success>
    {
        public Guid ShoppingCartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}