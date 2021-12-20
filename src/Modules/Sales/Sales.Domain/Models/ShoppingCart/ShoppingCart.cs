using NodaTime;
using System.Linq;
using System.Collections.Generic;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;

namespace VShop.Modules.Sales.Domain.Models.ShoppingCart
{
    public class ShoppingCart : AggregateRoot
    {
        private bool _isClosedForUpdates;
        private readonly List<ShoppingCartItem> _shoppingCartItems = new();
        
        public ShoppingCartCustomer Customer { get; private set; }
        public ShoppingCartStatus Status { get; private set; }
        public string PromoCode { get; private set; } // TODO - missing promo code implementation
        public Instant ConfirmedAt { get; private set; }
        public IReadOnlyList<ShoppingCartItem> Items => _shoppingCartItems;
        public Price DeliveryCost { get; private set; }
        public Price ProductsCostWithoutDiscount => new(_shoppingCartItems.Sum(sci => sci.TotalAmount));
        public Price TotalDiscount => ProductsCostWithoutDiscount * (Customer.Discount / 100.00m);
        public Price ProductsCostWithDiscount => ProductsCostWithoutDiscount - TotalDiscount;
        public Price FinalAmount => ProductsCostWithDiscount + DeliveryCost;
        public bool IsShoppingCartEmpty => _shoppingCartItems.Count is 0;
        public int TotalItemsCount => _shoppingCartItems.Count;
        public bool HasMinAmountForCheckout => ProductsCostWithDiscount >= Settings.MinShoppingCartAmountForCheckout;

        public Result Create
        (
            EntityId shoppingCartId,
            EntityId customerId,
            Discount customerDiscount
        )
        {
            RaiseEvent
            (
                new ShoppingCartCreatedDomainEvent
                {
                    ShoppingCartId = shoppingCartId,
                    CustomerId = customerId,
                    CustomerDiscount = customerDiscount
                }
            );

            return Result.Success;
        }
        
        public Result AddProduct(EntityId productId, ProductQuantity quantity, Price unitPrice)
        {
            if(_isClosedForUpdates)
                return Result.ValidationError($"Adding product for the shopping cart in '{Status}' status is not allowed.");

            ShoppingCartItem shoppingCartItem = _shoppingCartItems.SingleOrDefault(sci => sci.Id.Equals(productId));

            if (shoppingCartItem is null)
            {
                RaiseEvent
                (
                    new ShoppingCartProductAddedDomainEvent
                    {
                        ShoppingCartId = Id,
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = unitPrice
                    }
                );
            }
            else
            {
                if (!unitPrice.Equals(shoppingCartItem.UnitPrice))
                    return Result.ValidationError(@$"Product's quantity cannot be increased - shopping cart already contains the 
                                                requested product but with different unit price: {shoppingCartItem.UnitPrice}");

                Result increaseProductQuantityResult = shoppingCartItem.IncreaseProductQuantity(quantity);
                
                if (increaseProductQuantityResult.IsError(out ApplicationError error)) return error;
            }
            
            RecalculateDeliveryCost();

            return Result.Success;
        }
        
        public Result RemoveProduct(EntityId productId, ProductQuantity quantity)
        {
            if(_isClosedForUpdates)
                return Result.ValidationError($"Removing product from the shopping cart in '{Status}' status is not allowed.");
            
            ShoppingCartItem shoppingCartItem = FindShoppingCartItem(productId);

            if (shoppingCartItem is null)
                return Result.ValidationError($"Product with id `{productId}` was not found in shopping cart.");
            
            if (shoppingCartItem.Quantity - quantity <= 0)
            {
                RaiseEvent
                (
                    new ShoppingCartProductRemovedDomainEvent
                    {
                        ShoppingCartId = Id,
                        ProductId = productId
                    }
                );
            }
            else
            {
                Result decreaseProductQuantityResult = shoppingCartItem.DecreaseProductQuantity(quantity);

                if (decreaseProductQuantityResult.IsError(out ApplicationError error)) return error;
            }
            
            RecalculateDeliveryCost();
            
            return Result.Success;
        }
        
        public Result RequestCheckout(EntityId orderId, Instant now)
        {
            if(Status is not ShoppingCartStatus.AwaitingConfirmation)
                return Result.ValidationError($"Checkout is not allowed. Shopping cart Status: '{Status}'.");

            if(IsShoppingCartEmpty)
                return Result.ValidationError($"Checkout is not allowed. At least one product must be added in the shopping cart.");

            if(!HasMinAmountForCheckout)
                return Result.ValidationError(@$"Checkout is not allowed. Minimum required shopping cart amount 
                                                            for checkout is ${Settings.MinShoppingCartAmountForCheckout}.");
            RaiseEvent
            (
                new ShoppingCartCheckoutRequestedDomainEvent
                {
                    ShoppingCartId = Id,
                    OrderId = orderId,
                    ConfirmedAt = now
                }
            );
            
            return Result.Success;
        }
        
        public Result RequestDelete()
        {
            if (Status is ShoppingCartStatus.Closed)
                return Result.ValidationError($"Cannot proceed with the delete request. Shopping cart is already deleted/closed.");
            
            RaiseEvent(new ShoppingCartDeletionRequestedDomainEvent { ShoppingCartId = Id });
            
            return Result.Success;
        }

        private void RecalculateDeliveryCost()
        {
            decimal newDeliveryCost = (ProductsCostWithDiscount >= Settings.MinShoppingCartAmountForFreeDelivery) 
                ? 0 : Settings.DefaultDeliveryCost;
            
            if (newDeliveryCost != DeliveryCost)
                RaiseEvent
                (
                    new ShoppingCartDeliveryCostChangedDomainEvent
                    {
                        ShoppingCartId = Id,
                        DeliveryCost = newDeliveryCost
                    }
                );
        }

        private ShoppingCartItem FindShoppingCartItem(EntityId productId)
            => Items.SingleOrDefault(sci => sci.Id.Equals(productId));

        protected override void ApplyEvent(IDomainEvent @event)
        {
            ShoppingCartItem shoppingCartItem;
            
            switch (@event)
            {
                case ShoppingCartCreatedDomainEvent e:
                    Id = new EntityId(e.ShoppingCartId);
                    DeliveryCost = new Price(Settings.DefaultDeliveryCost);
                    Status = ShoppingCartStatus.New;

                    // one-to-one relationship
                    ShoppingCartCustomer shoppingCartCustomer = new(RaiseEvent);
                    ApplyToEntity(shoppingCartCustomer, e);
                    Customer = shoppingCartCustomer;
                    break;
                case ShoppingCartProductAddedDomainEvent e:
                    shoppingCartItem = new ShoppingCartItem(RaiseEvent);
                    ApplyToEntity(shoppingCartItem, e);
                    _shoppingCartItems.Add(shoppingCartItem);
                    break;
                case ShoppingCartProductRemovedDomainEvent e:
                    shoppingCartItem = FindShoppingCartItem(new EntityId(e.ProductId));
                    _shoppingCartItems.Remove(shoppingCartItem);
                    break;
                case ShoppingCartItemQuantityIncreasedDomainEvent e:
                    shoppingCartItem = FindShoppingCartItem(new EntityId(e.ProductId));
                    ApplyToEntity(shoppingCartItem, e);
                    break;
                case ShoppingCartItemQuantityDecreasedDomainEvent e:
                    shoppingCartItem = FindShoppingCartItem(new EntityId(e.ProductId));
                    ApplyToEntity(shoppingCartItem, e);
                    break;
                case ShoppingCartContactInformationSetDomainEvent e:
                    ApplyToEntity(Customer, e);
                    break;
                case ShoppingCartDeliveryAddressSetDomainEvent e:
                    ApplyToEntity(Customer, e);
                    Status = ShoppingCartStatus.AwaitingConfirmation;
                    break;
                case ShoppingCartDeliveryCostChangedDomainEvent e:
                    DeliveryCost = new Price(e.DeliveryCost);
                    break;
                case ShoppingCartCheckoutRequestedDomainEvent e:
                    ApplyToEntity(Customer, e);
                    Status = ShoppingCartStatus.PendingCheckout;
                    ConfirmedAt = e.ConfirmedAt;
                    _isClosedForUpdates = true;
                    break;
                case ShoppingCartDeletionRequestedDomainEvent _:
                    Status = ShoppingCartStatus.Closed;
                    break;
            }
        }

        // TODO - use settings from the database. Need to support admin pages.
        public static class Settings
        {
            public const decimal MinShoppingCartAmountForCheckout = 100m;
            public const decimal DefaultDeliveryCost = 20m;
            public const decimal MinShoppingCartAmountForFreeDelivery = 500m;
            public const int SalesTax = 25; // TODO - include receipt calculation
        }
    }
}