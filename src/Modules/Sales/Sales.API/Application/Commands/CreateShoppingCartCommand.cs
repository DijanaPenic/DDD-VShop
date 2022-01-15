using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class CreateShoppingCartCommand : Command<ShoppingCart>, IBaseCommand
    {
        public CreateShoppingCartCommand
        (
            Guid shoppingCartId,
            Guid customerId,
            int customerDiscount,
            IEnumerable<ShoppingCartItemCommand> shoppingCartItems,
            MessageMetadata metadata = default
        )
        {
            ShoppingCartId = shoppingCartId;
            CustomerId = customerId;
            CustomerDiscount = customerDiscount;
            ShoppingCartItems.AddRange(shoppingCartItems);
            Metadata = metadata;
        }
    }
}