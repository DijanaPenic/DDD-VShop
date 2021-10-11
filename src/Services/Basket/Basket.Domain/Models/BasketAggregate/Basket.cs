using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Domain;
using VShop.Services.Basket.Domain.Events;

namespace VShop.Services.Basket.Domain.Models.BasketAggregate
{
    public class Basket : AggregateRoot
    {
        private const decimal MinBasketAmountForCheckout = 100m;

        public EntityId CustomerId { get; private set; }

        public BasketStatus Status { get; private set; }
        
        private List<BasketItem> _basketItems;
        public IReadOnlyCollection<BasketItem> BasketItems => _basketItems;

        public int Discount { get; private set; }

        public decimal TotalAmountWithoutDiscount => _basketItems.Sum(bi => bi.TotalAmount);
        
        public decimal TotalDeduction => (Discount / 100.00m) * TotalAmountWithoutDiscount;
        
        public decimal FinalAmount => TotalAmountWithoutDiscount - TotalDeduction;

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

        public void AddProduct(EntityId productId, Quantity quantity, decimal unitPrice)
        {
            BasketItem basketItem = _basketItems.SingleOrDefault(bi => bi.ProductId.Equals(productId));
            if (basketItem != null)
            {
                if (unitPrice != basketItem.UnitPrice)
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
        }
        
        public void IncreaseProductQuantity(EntityId productId, Quantity value)
        {
            BasketItem basketItem = FindBasketItem(productId);

            if (basketItem == null)
                throw new InvalidOperationException("Requested product item cannot be found.");

            basketItem.IncreaseQuantity(value);
        }
        
        public void DecreaseProductQuantity(EntityId productId, Quantity value)
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
                basketItem.DecreaseQuantity(value);          
            }
        }

        private BasketItem FindBasketItem(EntityId productId)
            => BasketItems.SingleOrDefault(bi => bi.ProductId.Equals(productId));

        protected override void When(object @event)
        {
            BasketItem basketItem;
            
            switch (@event)
            {
                case BasketCreatedDomainEvent e:
                    Id = new EntityId(e.Id);
                    CustomerId = new EntityId(e.CustomerId);
                    Discount = e.Discount;
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
            }
        }
        
        public enum BasketStatus { New, PendingCheckout, Closed }
    }
}