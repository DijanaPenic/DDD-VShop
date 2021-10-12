using System;

using VShop.SharedKernel.EventSourcing;
using VShop.Services.Basket.Domain.Events;
using VShop.Services.Basket.Domain.Models.Shared;

namespace VShop.Services.Basket.Domain.Models.BasketAggregate
{
    public class BasketItem : Entity<EntityId>
    {
        private const int MaxQuantityPerProduct = 10;
        
        public EntityId ProductId { get; private set; }
        
        public ProductQuantity Quantity { get; private set; }
        
        public Price UnitPrice { get; private set; }

        public Price TotalAmount => UnitPrice * Quantity;

        public BasketItem(Action<object> applier) : base(applier) { }
        
        public void IncreaseProductQuantity(ProductQuantity value)
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
        
        public void DecreaseProductQuantity(ProductQuantity value)
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
                    Quantity = new ProductQuantity(e.Quantity);
                    UnitPrice = new Price(e.UnitPrice);
                    break;
                case BasketItemQuantityIncreasedDomainEvent e:
                    Quantity += new ProductQuantity(e.Quantity);
                    break;
                case BasketItemQuantityDecreasedDomainEvent e:
                    Quantity -= new ProductQuantity(e.Quantity);
                    break;
            }
        }
    }
}