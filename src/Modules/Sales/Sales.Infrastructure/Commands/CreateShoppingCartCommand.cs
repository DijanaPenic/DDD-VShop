using System;
using System.Collections.Generic;

using VShop.Modules.Sales.Infrastructure.Commands.Shared;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
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