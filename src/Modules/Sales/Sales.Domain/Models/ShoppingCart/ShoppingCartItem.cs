using System;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Events;

namespace VShop.Modules.Sales.Domain.Models.ShoppingCart
{
    public class ShoppingCartItem : Entity<EntityId>
    {
        public static class Settings
        {
            public const decimal MaxQuantityPerProduct = 10;
        }

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
                    ProductId = Id,
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
                    ProductId = Id,
                    Quantity = value
                }
            );
            
            return Option<ApplicationError>.None;
        }
        
        protected override void When(IDomainEvent @event)
        {
            switch (@event)
            {
                case ShoppingCartProductAddedDomainEvent e:
                    Id = new EntityId(e.ProductId);
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