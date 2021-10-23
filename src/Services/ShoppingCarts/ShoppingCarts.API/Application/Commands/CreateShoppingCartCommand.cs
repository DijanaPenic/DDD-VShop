﻿using System;
using OneOf.Types;

using VShop.SharedKernel.Application.Commands;
using VShop.Services.ShoppingCarts.API.Application.Commands.Shared;
using VShop.Services.ShoppingCarts.Domain.Models.ShoppingCartAggregate;

namespace VShop.Services.ShoppingCarts.API.Application.Commands
{
    public record CreateShoppingCartCommand : ICommand<Success<ShoppingCart>>
    {
        public Guid CustomerId { get; set; }
        public int CustomerDiscount { get; set; }
        public ShoppingCartItemDto[] ShoppingCartItems { get; set; }
    }
}