using System;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Sales.Infrastructure.Commands.Shared;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    internal partial class CreateShoppingCartCommand : ICommand
    {
        public CreateShoppingCartCommand
        (
            Guid shoppingCartId,
            Guid customerId,
            int customerDiscount,
            IEnumerable<ShoppingCartProductCommandDto> shoppingCartItems
        )
        {
            ShoppingCartId = shoppingCartId;
            CustomerId = customerId;
            CustomerDiscount = customerDiscount;
            ShoppingCartItems.AddRange(shoppingCartItems);
        }
    }
}