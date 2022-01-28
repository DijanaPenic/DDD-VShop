using System;

using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    public partial class PlaceOrderCommand : ICommand<Order>
    {
        public PlaceOrderCommand
        (
            Guid orderId,
            Guid shoppingCartId
        )
        {
            OrderId = orderId;
            ShoppingCartId = shoppingCartId;
        }
    }
}