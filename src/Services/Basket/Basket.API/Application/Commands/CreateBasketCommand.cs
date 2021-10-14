﻿using System;
using MediatR;

using VShop.Services.Basket.API.Application.Commands.Shared;

namespace VShop.Services.Basket.API.Application.Commands
{
    public record CreateBasketCommand : IRequest<bool>
    {
        public Guid CustomerId { get; set; }
        public int CustomerDiscount { get; set; }
        public BasketItemDto[] BasketItems { get; set; }
    }
}