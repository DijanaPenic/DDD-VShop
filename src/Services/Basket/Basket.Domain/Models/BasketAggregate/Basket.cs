using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.EventSourcing;
using VShop.Services.Basket.Domain.Events;
using VShop.Services.Basket.Domain.Models.Shared;

namespace VShop.Services.Basket.Domain.Models.BasketAggregate
{
    public class Basket : AggregateRoot<EntityId>
    {
        public static class Settings
        {
            public const decimal MinBasketAmountForCheckout = 100m;
            public const decimal DefaultDeliveryCost = 20m;
            public const decimal MinBasketAmountForFreeDelivery = 500m;
            public const int SalesTax = 25; // TODO - include receipt calculation
        }

        public BasketCustomer BasketCustomer { get; private set; }

        public BasketStatus Status { get; private set; }

        public string PromoCode { get; private set; } // TODO - missing promo code implementation

        public DateTime ConfirmedAt { get; private set; }
        
        private List<BasketItem> _basketItems;
        public IReadOnlyCollection<BasketItem> BasketItems => _basketItems;

        public Price DeliveryCost { get; private set; }

        public Price ProductsCostWithoutDiscount => new(_basketItems.Sum(bi => bi.TotalAmount));
        
        public Price TotalDeduction => ProductsCostWithoutDiscount * (BasketCustomer.Discount / 100.00m);
        
        public Price ProductsCostWithDiscount => ProductsCostWithoutDiscount - TotalDeduction;

        public Price FinalAmount => ProductsCostWithDiscount + DeliveryCost;
        
        public bool IsBasketEmpty => _basketItems.Count == 0;
        
        public int TotalItemsCount() => _basketItems.Count;

        public static Basket Create(EntityId customerId, int customerDiscount)
        {
            Basket basket = new();
            
            basket.Apply
            (
                new BasketCreatedDomainEvent
                {
                    BasketId = Guid.NewGuid(), // TODO - sequential guid
                    CustomerId = customerId,
                    CustomerDiscount = customerDiscount
                }
            );

            return basket;
        }

        public void AddProduct(EntityId productId, ProductQuantity quantity, Price unitPrice)
        {
            if(Status != BasketStatus.Fulfilled && Status != BasketStatus.New)
                throw new InvalidOperationException($"Adding product for the cart in '{Status}' status is not allowed.");
            
            BasketItem basketItem = _basketItems.SingleOrDefault(bi => bi.ProductId.Equals(productId));

            if (basketItem == null)
            {
                Apply
                (
                    new ProductAddedToBasketDomainEvent
                    {
                        BasketId = Id,
                        BasketItemId = Guid.NewGuid(),
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = unitPrice
                    }
                );
            }
            else
            {
                if (!unitPrice.Equals(basketItem.UnitPrice))
                    throw new Exception($"Product's quantity cannot be adjusted - basket already contains the requested product but with different unit price: {basketItem.UnitPrice}");

                basketItem.IncreaseProductQuantity(quantity);
            }
            
            RecalculateDeliveryCost();
        }
        
        public void RemoveProduct(EntityId productId)
        {
            if(Status != BasketStatus.Fulfilled && Status != BasketStatus.New)
                throw new InvalidOperationException($"Removing product from the cart in '{Status}' status is not allowed.");
            
            BasketItem basketItem = FindBasketItem(productId);

            if (basketItem == null)
                throw new InvalidOperationException($"Product with id `{productId}` was not found in cart.");
            
            Apply
            (
                new ProductRemovedFromBasketDomainEvent
                {
                    BasketId = Id,
                    ProductId = productId
                }
            );
            
            RecalculateDeliveryCost();
        }
        
        public void IncreaseProductQuantity(EntityId productId, ProductQuantity value)
        {
            if(Status != BasketStatus.Fulfilled && Status != BasketStatus.New)
                throw new InvalidOperationException($"Updating product for the cart in '{Status}' status is not allowed.");
            
            BasketItem basketItem = FindBasketItem(productId);

            if (basketItem == null)
                throw new InvalidOperationException($"Product with id `{productId}` was not found in cart.");

            basketItem.IncreaseProductQuantity(value);
            
            RecalculateDeliveryCost();
        }
        
        public void DecreaseProductQuantity(EntityId productId, ProductQuantity value)
        {
            if(Status != BasketStatus.Fulfilled && Status != BasketStatus.New)
                throw new InvalidOperationException($"Updating product for the cart in '{Status}' status is not allowed.");
             
            BasketItem basketItem = FindBasketItem(productId);

            if (basketItem == null)
                throw new InvalidOperationException($"Product with id `{productId}` was not found in cart.");

            if (basketItem.Quantity - value <= 0)
            {
                RemoveProduct(productId);
            }
            else
            {
                basketItem.DecreaseProductQuantity(value);          
            }
            
            RecalculateDeliveryCost();
        }

        public void RequestCheckout()
        {
            if(Status != BasketStatus.Fulfilled)
                throw new InvalidOperationException($"Checkout is not allowed. Basket Status: '{Status}'.");

            if(IsBasketEmpty)
                throw new InvalidOperationException($"Checkout is not allowed. At least one product must be added in the basket.");

            if(ProductsCostWithDiscount < Settings.MinBasketAmountForCheckout)
                throw new InvalidOperationException($"Checkout is not allowed. Minimum required basket amount for checkout is ${Settings.MinBasketAmountForCheckout}.");

            Apply
            (
                new BasketCheckoutRequestedDomainEvent
                {
                    BasketId = Id,
                    ConfirmedAt = DateTime.UtcNow
                }
            );
        }
        
        public void RequestDelete()
        {
            if (Status == BasketStatus.Closed)
                throw new InvalidOperationException($"Cannot proceed with the delete request. Basket is already deleted/closed.");
            
            Apply
            (
                new BasketDeletionRequestedDomainEvent
                {
                    BasketId = Id
                }
            );
        }

        private void RecalculateDeliveryCost()
        {
            decimal newDeliveryCost = (ProductsCostWithDiscount >= Settings.MinBasketAmountForFreeDelivery) ? 0 : Settings.DefaultDeliveryCost;
            
            if (newDeliveryCost != DeliveryCost)
                Apply
                (
                    new DeliveryCostChangedDomainEvent
                    {
                        BasketId = Id,
                        DeliveryCost = newDeliveryCost
                    }
                );
        }

        private BasketItem FindBasketItem(EntityId productId)
            => BasketItems.SingleOrDefault(bi => bi.ProductId.Equals(productId));

        protected override void When(object @event)
        {
            BasketItem basketItem;
            
            switch (@event)
            {
                case BasketCreatedDomainEvent e:
                    Id = new EntityId(e.BasketId);

                    BasketCustomer basketCustomer = new(Apply);
                    ApplyToEntity(basketCustomer, e);
                    BasketCustomer = basketCustomer;
                    
                    
                    DeliveryCost = new Price(Settings.DefaultDeliveryCost);
                    Status = BasketStatus.New;
                    _basketItems = new List<BasketItem>();
                    break;
                case ProductAddedToBasketDomainEvent e:
                    basketItem = new BasketItem(Apply);
                    ApplyToEntity(basketItem, e);
                    _basketItems.Add(basketItem);
                    break;
                case ProductRemovedFromBasketDomainEvent e:
                    basketItem = FindBasketItem(new EntityId(e.ProductId));
                    _basketItems.Remove(basketItem);
                    break;
                case DeliveryAddressSetDomainEvent e:
                    Status = BasketStatus.Fulfilled;
                    break;
                case DeliveryCostChangedDomainEvent e:
                    DeliveryCost = new Price(e.DeliveryCost);
                    break;
                case BasketCheckoutRequestedDomainEvent e:
                    Status = BasketStatus.PendingCheckout;
                    ConfirmedAt = e.ConfirmedAt;
                    break;
                case BasketDeletionRequestedDomainEvent _:
                    Status = BasketStatus.Closed;
                    break;
            }
        }

        public enum BasketStatus
        {
            New,
            Fulfilled,              // customer has provided needed contact information and can proceed checkout
            PendingCheckout,        // ready to pay (once inventory is checked)
            Closed
        }
    }
}