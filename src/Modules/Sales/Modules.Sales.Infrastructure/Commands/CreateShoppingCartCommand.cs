using System;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Sales.Infrastructure.Commands.Shared;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    public partial class CreateShoppingCartCommand : MessageContext, ICommand
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