using System;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class CreateShoppingCartCommand : MessageContext, ICommand<ShoppingCart>
    {
        public CreateShoppingCartCommand
        (
            Guid shoppingCartId,
            Guid customerId,
            int customerDiscount,
            IEnumerable<ShoppingCartProductCommandDto> shoppingCartItems,
            MessageMetadata metadata
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