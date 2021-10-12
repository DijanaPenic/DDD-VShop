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
        
        private List<BasketItem> _basketItems;
        public IReadOnlyCollection<BasketItem> BasketItems => _basketItems;

        public int Discount { get; private set; }

        public Price DeliveryCost { get; private set; }

        public Price ProductCostWithoutDiscount => new(_basketItems.Sum(bi => bi.TotalAmount));
        
        public Price TotalDeduction => ProductCostWithoutDiscount * (Discount / 100.00m) ;
        
        public Price ProductCostWithDiscount => ProductCostWithoutDiscount - TotalDeduction;

        public Price FinalAmount => ProductCostWithDiscount + DeliveryCost;

        public static Basket Create(EntityId customerId, int discount)
        {
            Basket basket = new();
            
            basket.Apply
            (
                new BasketCreatedDomainEvent
                {
                    Id = Guid.NewGuid(), // TODO - sequential guid
                    CustomerId = customerId,
                    Discount = discount
                }
            );

            return basket;
        }

        public void AddProduct(EntityId productId, ProductQuantity quantity, Price unitPrice)
        {
            BasketItem basketItem = _basketItems.SingleOrDefault(bi => bi.ProductId.Equals(productId));
            if (basketItem != null)
            {
                if (!unitPrice.Equals(basketItem.UnitPrice))
                    throw new Exception($"Basket contains product but with different unit price: {basketItem.UnitPrice}");
            }
            
            Apply
            (
                new ProductAddedToBasketDomainEvent
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                }
            );

            RecalculateDeliveryCost();
        }
        
        public void RemoveProduct(EntityId productId)
        {
            BasketItem basketItem = FindBasketItem(productId);

            if (basketItem == null)
                throw new InvalidOperationException("Requested product item cannot be found.");
            
            Apply
            (
                new ProductRemovedFromBasketDomainEvent
                {
                    ProductId = productId
                }
            );
            
            RecalculateDeliveryCost();
        }
        
        public void IncreaseProductQuantity(EntityId productId, ProductQuantity value)
        {
            BasketItem basketItem = FindBasketItem(productId);

            if (basketItem == null)
                throw new InvalidOperationException("Requested product item cannot be found.");

            basketItem.IncreaseProductQuantity(value);
            
            RecalculateDeliveryCost();
        }
        
        public void DecreaseProductQuantity(EntityId productId, ProductQuantity value)
        {
            BasketItem basketItem = FindBasketItem(productId);

            if (basketItem == null)
                throw new InvalidOperationException("Requested product item cannot be found.");

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

        private void RecalculateDeliveryCost()
        {
            decimal newDeliveryCost = (ProductCostWithDiscount >= Settings.MinBasketAmountForFreeDelivery) ? 0 : Settings.DefaultDeliveryCost;
            
            if (newDeliveryCost != DeliveryCost)
                Apply
                (
                    new DeliveryCostChangedDomainEvent
                    {
                        DeliveryCost = newDeliveryCost
                    }
                );
        }
        
        private BasketItem FindBasketItem(EntityId productId)
            => BasketItems.SingleOrDefault(bi => bi.ProductId.Equals(productId));

        protected override void When(object @event)
        {
            BasketItem basketItem;
            BasketCustomer basketCustomer;
            
            switch (@event)
            {
                case BasketCreatedDomainEvent e:
                    Id = new EntityId(e.Id);

                    basketCustomer = new BasketCustomer(Apply);
                    ApplyToEntity(basketCustomer, e);
                    BasketCustomer = basketCustomer;
                    

                    Discount = e.Discount;
                    DeliveryCost = new Price(0);
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
                case DeliveryCostChangedDomainEvent e:
                    DeliveryCost = new Price(e.DeliveryCost);
                    break;
            }
        }
        
        public enum BasketStatus { New, PendingCheckout, Closed }
    }
}