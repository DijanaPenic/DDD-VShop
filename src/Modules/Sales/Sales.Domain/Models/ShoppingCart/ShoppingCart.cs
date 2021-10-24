using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Domain;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.Modules.Sales.Domain.Events;

namespace VShop.Modules.Sales.Domain.Models.ShoppingCart
{
    public class ShoppingCart : AggregateRoot<EntityId>
    {
        private bool _isClosedForUpdates;
        private List<ShoppingCartItem> _shoppingCartItems;
        
        public ShoppingCartCustomer Customer { get; private set; }
        public ShoppingCartStatus Status { get; private set; }
        public string PromoCode { get; private set; } // TODO - missing promo code implementation
        public DateTime ConfirmedAt { get; private set; }
        public IReadOnlyCollection<ShoppingCartItem> Items => _shoppingCartItems;
        public Price DeliveryCost { get; private set; }
        public Price ProductsCostWithoutDiscount => new(_shoppingCartItems.Sum(sci => sci.TotalAmount));
        public Price TotalDeduction => ProductsCostWithoutDiscount * (Customer.Discount / 100.00m);
        public Price ProductsCostWithDiscount => ProductsCostWithoutDiscount - TotalDeduction;
        public Price FinalAmount => ProductsCostWithDiscount + DeliveryCost;
        public bool IsShoppingCartEmpty => _shoppingCartItems.Count == 0;
        public int TotalItemsCount() => _shoppingCartItems.Count;

        public static ShoppingCart Create(EntityId customerId, int customerDiscount)
        {
            ShoppingCart shoppingCart = new();
            
            shoppingCart.Apply
            (
                new ShoppingCartCreatedDomainEvent
                {
                    ShoppingCartId = GuidHelper.NewSequentialGuid(),
                    CustomerId = customerId,
                    CustomerDiscount = customerDiscount
                }
            );

            return shoppingCart;
        }
        
        public Option<ApplicationError> AddProduct(EntityId productId, ProductQuantity quantity, Price unitPrice)
        {
            if(_isClosedForUpdates)
                return ValidationError.Create($"Adding product for the shopping cart in '{Status}' status is not allowed.");

            ShoppingCartItem shoppingCartItem = _shoppingCartItems.SingleOrDefault(sci => sci.ProductId.Equals(productId));

            if (shoppingCartItem == null)
            {
                Apply
                (
                    new ProductAddedToShoppingCartDomainEvent
                    {
                        ShoppingCartId = Id,
                        ShoppingCartItemId = GuidHelper.NewSequentialGuid(),
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = unitPrice
                    }
                );
            }
            else
            {
                if (!unitPrice.Equals(shoppingCartItem.UnitPrice))
                    return ValidationError.Create(@$"Product's quantity cannot be increased - shopping cart already contains the 
                                                requested product but with different unit price: {shoppingCartItem.UnitPrice}");

                Option<ApplicationError> errorResult = shoppingCartItem.IncreaseProductQuantity(quantity);
                
                if (errorResult.IsSome(out ApplicationError error)) return error;
            }
            
            RecalculateDeliveryCost();

            return Option<ApplicationError>.None;
        }
        
        public Option<ApplicationError> RemoveProduct(EntityId productId, ProductQuantity quantity)
        {
            if(_isClosedForUpdates)
                return ValidationError.Create($"Removing product from the shopping cart in '{Status}' status is not allowed.");
            
            ShoppingCartItem shoppingCartItem = FindShoppingCartItem(productId);

            if (shoppingCartItem == null)
                return ValidationError.Create($"Product with id `{productId}` was not found in shopping cart.");
            
            if (shoppingCartItem.Quantity - quantity <= 0)
            {
                Apply
                (
                    new ProductRemovedFromShoppingCartDomainEvent
                    {
                        ShoppingCartId = Id,
                        ProductId = productId
                    }
                );
            }
            else
            {
                Option<ApplicationError> errorResult = shoppingCartItem.DecreaseProductQuantity(quantity);

                if (errorResult.IsSome(out ApplicationError error)) return error;
            }
            
            RecalculateDeliveryCost();
            
            return Option<ApplicationError>.None;
        }

        public Option<ApplicationError> RequestCheckout()
        {
            if(Status != ShoppingCartStatus.AwaitingConfirmation)
                return ValidationError.Create($"Checkout is not allowed. Shopping cart Status: '{Status}'.");

            if(IsShoppingCartEmpty)
                return ValidationError.Create($"Checkout is not allowed. At least one product must be added in the shopping cart.");

            if(ProductsCostWithDiscount < Settings.MinShoppingCartAmountForCheckout)
                return ValidationError.Create(@$"Checkout is not allowed. Minimum required shopping cart amount 
                                                            for checkout is ${Settings.MinShoppingCartAmountForCheckout}.");
            Apply
            (
                new ShoppingCartCheckoutRequestedDomainEvent
                {
                    ShoppingCartId = Id,
                    ConfirmedAt = DateTime.UtcNow
                }
            );
            
            return Option<ApplicationError>.None;
        }
        
        public Option<ApplicationError> RequestDelete()
        {
            if (Status == ShoppingCartStatus.Closed)
                return ValidationError.Create($"Cannot proceed with the delete request. Shopping cart is already deleted/closed.");
            
            Apply
            (
                new ShoppingCartDeletionRequestedDomainEvent
                {
                    ShoppingCartId = Id
                }
            );
            
            return Option<ApplicationError>.None;
        }

        private void RecalculateDeliveryCost()
        {
            decimal newDeliveryCost = (ProductsCostWithDiscount >= Settings.MinShoppingCartAmountForFreeDelivery) ? 0 : Settings.DefaultDeliveryCost;
            
            if (newDeliveryCost != DeliveryCost)
                Apply
                (
                    new DeliveryCostChangedDomainEvent
                    {
                        ShoppingCartId = Id,
                        DeliveryCost = newDeliveryCost
                    }
                );
        }

        private ShoppingCartItem FindShoppingCartItem(EntityId productId)
            => Items.SingleOrDefault(sci => sci.ProductId.Equals(productId));

        protected override void When(IDomainEvent @event)
        {
            ShoppingCartItem shoppingCartItem;
            
            switch (@event)
            {
                case ShoppingCartCreatedDomainEvent e:
                    Id = new EntityId(e.ShoppingCartId);

                    // one-to-one relationship
                    ShoppingCartCustomer shoppingCartCustomer = new(Apply);
                    ApplyToEntity(shoppingCartCustomer, e);
                    Customer = shoppingCartCustomer;
                    
                    DeliveryCost = new Price(Settings.DefaultDeliveryCost);
                    Status = ShoppingCartStatus.New;
                    
                    // one-to-many relationship
                    _shoppingCartItems = new List<ShoppingCartItem>();
                    break;
                case ProductAddedToShoppingCartDomainEvent e:
                    shoppingCartItem = new ShoppingCartItem(Apply);
                    ApplyToEntity(shoppingCartItem, e);
                    _shoppingCartItems.Add(shoppingCartItem);
                    break;
                case ProductRemovedFromShoppingCartDomainEvent e:
                    shoppingCartItem = FindShoppingCartItem(new EntityId(e.ProductId));
                    _shoppingCartItems.Remove(shoppingCartItem);
                    break;
                case DeliveryAddressSetDomainEvent _:
                    Status = ShoppingCartStatus.AwaitingConfirmation;
                    break;
                case DeliveryCostChangedDomainEvent e:
                    DeliveryCost = new Price(e.DeliveryCost);
                    break;
                case ShoppingCartCheckoutRequestedDomainEvent e:
                    Status = ShoppingCartStatus.PendingCheckout;
                    ConfirmedAt = e.ConfirmedAt;
                    _isClosedForUpdates = true;
                    break;
                case ShoppingCartDeletionRequestedDomainEvent _:
                    Status = ShoppingCartStatus.Closed;
                    break;
            }
        }

        public enum ShoppingCartStatus
        {
            New = 1,
            AwaitingConfirmation = 2,     // Customer has provided needed contact information and is allowed to proceed with checkout.
            PendingCheckout = 3,          // Checkout has been requested. The next step: payment.
            Closed = 4                    // Shopping cart has been deleted (soft delete).
        }
        
        public static class Settings
        {
            public const decimal MinShoppingCartAmountForCheckout = 100m;
            public const decimal DefaultDeliveryCost = 20m;
            public const decimal MinShoppingCartAmountForFreeDelivery = 500m;
            public const int SalesTax = 25; // TODO - include receipt calculation
        }
    }
}