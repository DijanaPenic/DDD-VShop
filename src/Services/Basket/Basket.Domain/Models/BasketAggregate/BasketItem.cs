﻿using System;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Domain;
using VShop.SharedKernel.Infrastructure.Domain.ValueObjects;
using VShop.Services.Basket.Domain.Events;

namespace VShop.Services.Basket.Domain.Models.BasketAggregate
{
    public class BasketItem : Entity<EntityId>
    {
        public static class Settings
        {
            public const decimal MaxQuantityPerProduct = 10;
        }
        
        public EntityId BasketId { get; private set; }
        
        public EntityId ProductId { get; private set; }
        
        public ProductQuantity Quantity { get; private set; }
        
        public Price UnitPrice { get; private set; }

        public Price TotalAmount => UnitPrice * Quantity;

        public BasketItem(Action<IDomainEvent> applier) : base(applier) { }
        
        public void IncreaseProductQuantity(ProductQuantity value)
        {
            if (Quantity + value > Settings.MaxQuantityPerProduct)
                throw new Exception($"Maximum allowed quantity per single product is {Settings.MaxQuantityPerProduct}.");
            
            Apply
            (
                new BasketItemQuantityIncreasedDomainEvent
                {
                    BasketId = BasketId,
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
                    BasketId = BasketId,
                    ProductId = ProductId,
                    Quantity = value
                }
            );
        }
        
        protected override void When(IDomainEvent @event)
        {
            switch (@event)
            {
                case ProductAddedToBasketDomainEvent e:
                    Id = new EntityId(e.BasketItemId);
                    BasketId = new EntityId(e.BasketId);
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