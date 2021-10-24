﻿using System;
using OneOf.Types;

using VShop.SharedKernel.Application.Commands;

namespace VShop.Services.ShoppingCarts.API.Application.Commands
{
    public record CheckoutShoppingCartCommand : ICommand<Success>
    {
        public Guid ShoppingCartId { get; set; }
    }
}