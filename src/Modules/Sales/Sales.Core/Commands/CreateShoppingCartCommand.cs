using System;
using System.Collections.Generic;

using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Core.Commands.Shared;
using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Core.Commands
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