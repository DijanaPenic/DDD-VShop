﻿using System;

using VShop.Modules.Sales.Domain.Events;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;

namespace VShop.Modules.Sales.Domain.Models.ShoppingCart
{
    public class ShoppingCartItem : Entity<EntityId>
    {
        public EntityId ShoppingCartId { get; private set; }

        public ProductQuantity Quantity { get; private set; }
        
        public Price UnitPrice { get; private set; }

        public Price TotalAmount => UnitPrice * Quantity;

        public ShoppingCartItem(Action<IDomainEvent> applier) : base(applier) { }
        
        public Result IncreaseQuantity(ProductQuantity value)
        {
            if (Quantity + value > Settings.MaxQuantityPerProduct)
                return Result.ValidationError($"Maximum allowed quantity per single product is {Settings.MaxQuantityPerProduct}.");
            
            RaiseEvent
            (
                new ShoppingCartItemQuantityIncreasedDomainEvent
                (
                    ShoppingCartId,
                    Id,
                    value
                )
            );
            
            return Result.Success;
        }
        
        public Result DecreaseQuantity(ProductQuantity value)
        {
            if (Quantity - value <= 0)
                return Result.ValidationError($"Cannot decrease quantity by {value}.");
            
            RaiseEvent
            (
                new ShoppingCartItemQuantityDecreasedDomainEvent
                (
                    ShoppingCartId,
                    Id,
                    value
                )
            );
            
            return Result.Success;
        }
        
        protected override void ApplyEvent(IDomainEvent @event)
        {
            switch (@event)
            {
                case ShoppingCartProductAddedDomainEvent e:
                    Id = new EntityId(e.ProductId);
                    ShoppingCartId = new EntityId(e.ShoppingCartId);
                    Quantity = new ProductQuantity(e.Quantity);
                    UnitPrice = new Price(e.UnitPrice);
                    break;
                case ShoppingCartItemQuantityIncreasedDomainEvent e:
                    Quantity += new ProductQuantity(e.Quantity);
                    break;
                case ShoppingCartItemQuantityDecreasedDomainEvent e:
                    Quantity -= new ProductQuantity(e.Quantity);
                    break;
            }
        }
        
        public static class Settings
        {
            public const int MaxQuantityPerProduct = 10;
        }
    }
}