using System;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Domain;
using VShop.Services.Basket.Domain.Events;

namespace VShop.Services.Basket.Domain.Models.BasketAggregate
{
    public class BasketItem : Entity
    {
        private const int MaxQuantityPerProduct = 10;
        
        public EntityId ProductId { get; private set; }
        
        public Quantity Quantity { get; private set; }
        
        public decimal UnitPrice { get; private set; }

        public decimal TotalAmount => UnitPrice * Quantity;

        public BasketItem(Action<object> applier) : base(applier) { }
        
        public void IncreaseQuantity(Quantity value)
        {
            if (Quantity + value > MaxQuantityPerProduct)
                throw new Exception($"Maximum allowed quantity per single product is {MaxQuantityPerProduct}.");
            
            Apply
            (
                new BasketItemQuantityIncreasedDomainEvent
                {
                    ProductId = ProductId,
                    Quantity = value
                }
            );
        }
        
        public void DecreaseQuantity(Quantity value)
        {
            if (Quantity - value <= 0)
                throw new Exception($"Cannot decrease quantity by {value}.");
            
            Apply
            (
                new BasketItemQuantityDecreasedDomainEvent
                {
                    ProductId = ProductId,
                    Quantity = value
                }
            );
        }
        
        protected override void When(object @event)
        {
            switch (@event)
            {
                case ProductAddedToBasketDomainEvent e:
                    ProductId = new EntityId(e.ProductId);
                    Quantity = new Quantity(e.Quantity);
                    UnitPrice = e.UnitPrice;
                    break;
                case BasketItemQuantityIncreasedDomainEvent e:
                    Quantity += new Quantity(e.Quantity);
                    break;
                case BasketItemQuantityDecreasedDomainEvent e:
                    Quantity -= new Quantity(e.Quantity);
                    break;
            }
        }
    }
}