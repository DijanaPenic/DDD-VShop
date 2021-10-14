using System;
using MediatR;

namespace VShop.Services.Basket.API.Application.Commands
{
    public record CreateBasketCommand : IRequest<bool>
    {
        public Guid CustomerId { get; set; }
        public int CustomerDiscount { get; set; }
        public BasketItem[] BasketItems { get; set; }
        
        public record BasketItem
        {
            public Guid ProductId { get; set; }
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
        }
    }
}