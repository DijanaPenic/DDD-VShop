﻿using System;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Domain;
using VShop.SharedKernel.Infrastructure.Domain.ValueObjects;
using VShop.Services.ShoppingCarts.Domain.Events;

namespace VShop.Services.ShoppingCarts.Domain.Models.ShoppingCartAggregate
{
    public class ShoppingCartItem : Entity<EntityId>
    {
        public static class Settings
        {
            public const decimal MaxQuantityPerProduct = 10;
        }
        
        public EntityId ShoppingCartId { get; private set; }
        
        public EntityId ProductId { get; private set; }
        
        public ProductQuantity Quantity { get; private set; }
        
        public Price UnitPrice { get; private set; }

        public Price TotalAmount => UnitPrice * Quantity;

        public ShoppingCartItem(Action<IDomainEvent> applier) : base(applier) { }
        
        public Option<ApplicationError> IncreaseProductQuantity(ProductQuantity value)
        {
            if (Quantity + value > Settings.MaxQuantityPerProduct)
                return ValidationError.Create($"Maximum allowed quantity per single product is {Settings.MaxQuantityPerProduct}.");
            
            Apply
            (
                new ShoppingCartItemQuantityIncreasedDomainEvent
                {
                    ShoppingCartId = ShoppingCartId,
                    ProductId = ProductId,
                    Quantity = value
                }
            );
            
            return Option<ApplicationError>.None;
        }
        
        public Option<ApplicationError> DecreaseProductQuantity(ProductQuantity value)
        {
            if (Quantity - value <= 0)
                return ValidationError.Create($"Cannot decrease quantity by {value}.");
            
            Apply
            (
                new ShoppingCartItemQuantityDecreasedDomainEvent
                {
                    ShoppingCartId = ShoppingCartId,
                    ProductId = ProductId,
                    Quantity = value
                }
            );
            
            return Option<ApplicationError>.None;
        }
        
        protected override void When(IDomainEvent @event)
        {
            switch (@event)
            {
                case ProductAddedToShoppingCartDomainEvent e:
                    Id = new EntityId(e.ShoppingCartItemId);
                    ShoppingCartId = new EntityId(e.ShoppingCartId);
                    ProductId = new EntityId(e.ProductId);
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
    }
}