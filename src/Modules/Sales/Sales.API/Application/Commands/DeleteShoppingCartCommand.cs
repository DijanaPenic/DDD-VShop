﻿using System;
using OneOf.Types;

using VShop.SharedKernel.Application.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public record DeleteShoppingCartCommand : ICommand<Success>
    {
        public Guid ShoppingCartId { get; set; }
    }
}