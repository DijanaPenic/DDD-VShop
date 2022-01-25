﻿using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.Application.ValidationAttributes;

namespace VShop.Modules.Sales.API.Models
{
    internal record SetShoppingCartProductPriceRequest
    {
        [Required, Price]
        public decimal UnitPrice { get; init; }
    }
}