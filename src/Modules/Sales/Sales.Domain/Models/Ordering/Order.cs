using System;
using System.Linq;
using System.Collections.Generic;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;

namespace VShop.Modules.Sales.Domain.Models.Ordering
{
    public class Order : AggregateRoot
    {
        private readonly List<OrderLine> _orderLines = new();

        public int TotalOrderLineCount => _orderLines.Count;
        public Price DeliveryCost { get; private set; }
        public Price ProductsCostWithoutDiscount => new(_orderLines.Sum(ol => ol.TotalAmount));
        public Price TotalDiscount => ProductsCostWithoutDiscount * (Customer.Discount / 100.00m);
        public Price ProductsCostWithDiscount => ProductsCostWithoutDiscount - TotalDiscount;
        public Price FinalAmount => ProductsCostWithDiscount + DeliveryCost;
        public IReadOnlyList<OrderLine> OrderLines => _orderLines;
        public OrderStatus Status { get; private set; }
        public OrderCustomer Customer { get; private set; }
        
        public Order(Guid causationId, Guid correlationId) : base(causationId, correlationId) { }

        public static Result<Order> Create
        (
            EntityId orderId,
            Price deliveryCost,
            EntityId customerId,
            Discount customerDiscount,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            Address deliveryAddress,
            Guid causationId,
            Guid correlationId
        )
        {
            Order order = new(causationId, correlationId);
            
            order.RaiseEvent
            (
                new OrderPlacedDomainEvent
                {
                    OrderId = orderId,
                    DeliveryCost = deliveryCost,
                    CustomerDiscount = customerDiscount,
                    CustomerId = customerId,
                    FirstName = fullName.FirstName,
                    MiddleName = fullName.MiddleName,
                    LastName = fullName.LastName,
                    EmailAddress = emailAddress,
                    PhoneNumber = phoneNumber,
                    City = deliveryAddress.City,
                    CountryCode = deliveryAddress.CountryCode,
                    PostalCode = deliveryAddress.PostalCode,
                    StateProvince = deliveryAddress.StateProvince,
                    StreetAddress = deliveryAddress.StreetAddress
                }
            );

            return order;
        }

        public Result AddOrderLine
        (
            EntityId productId,
            ProductQuantity quantity,
            Price unitPrice
        )
        {
            RaiseEvent
            (
                new OrderLineAddedDomainEvent
                {
                    OrderId = Id,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                }
            );

            return Result.Success;
        }
        
        public Result RemoveOutOfStockItems(EntityId productId, ProductQuantity quantity)
        {
            OrderLine orderLine = FindOrderLine(productId);
            
            if (orderLine is null)
                return Result.ValidationError($"Product with id `{productId}` was not found in the order.");
            
            Result removeOutOfStockResult = orderLine.RemoveOutOfStockItems(quantity);
            if (removeOutOfStockResult.IsError) return removeOutOfStockResult.Error;

            return Result.Success;
        }

        public Result SetPaidStatus()
        {
            if(Status is not OrderStatus.Processing)
                return Result.ValidationError($"Changing status to '{OrderStatus.Paid}' is not allowed. Order Status: '{Status}'.");
            
            RaiseEvent(new OrderStatusSetToPaidDomainEvent(Id));
            
            return Result.Success;
        }

        public Result SetPendingShippingStatus()
        {
            if(Status is not OrderStatus.Paid)
                return Result.ValidationError($"Changing status to '{OrderStatus.PendingShipping}' is not allowed. Order Status: '{Status}'.");
            
            RaiseEvent(new OrderStatusSetToPendingShippingDomainEvent(Id));
            
            return Result.Success;
        }

        public Result SetShippedStatus()
        {
            if(Status is not OrderStatus.PendingShipping)
                return Result.ValidationError($"Changing status to '{OrderStatus.Shipped}' is not allowed. Order Status: '{Status}'.");
            
            RaiseEvent(new OrderStatusSetToShippedDomainEvent(Id));
            
            return Result.Success;
        }
        
        public Result SetCancelledStatus()
        {
            if(Status is not (OrderStatus.Processing or OrderStatus.Paid))
                return Result.ValidationError($"Changing status to '{OrderStatus.Cancelled}' is not allowed. Order Status: '{Status}'.");
            
            RaiseEvent(new OrderStatusSetToCancelledDomainEvent(Id));
            
            return Result.Success;
        }
        
        private OrderLine FindOrderLine(EntityId productId)
            => OrderLines.SingleOrDefault(ol => ol.Id.Equals(productId));

        protected override void ApplyEvent(IDomainEvent @event)
        {
            OrderLine orderLine;
            
            switch (@event)
            {
                case OrderPlacedDomainEvent e:
                    Id = new EntityId(e.OrderId);
                    DeliveryCost = new Price(e.DeliveryCost);
                    Status = OrderStatus.Processing;

                    // one-to-one relationship
                    OrderCustomer orderCustomer = new(RaiseEvent);
                    ApplyToEntity(orderCustomer, e);
                    Customer = orderCustomer;
                    break;
                case OrderLineAddedDomainEvent e:
                    orderLine = new OrderLine(RaiseEvent);
                    ApplyToEntity(orderLine, e);
                    _orderLines.Add(orderLine);
                    break;
                case OrderStatusSetToCancelledDomainEvent _:
                    Status = OrderStatus.Cancelled;
                    break;
                case OrderStatusSetToPendingShippingDomainEvent _:
                    Status = OrderStatus.PendingShipping;
                    break;
                case OrderStatusSetToShippedDomainEvent _:
                    Status = OrderStatus.Shipped;
                    break;
                case OrderStatusSetToPaidDomainEvent _:
                    Status = OrderStatus.Paid;
                    break;
                case OrderLineOutOfStockRemovedDomainEvent e:
                    orderLine = FindOrderLine(new EntityId(e.ProductId));
                    ApplyToEntity(orderLine, e);
                    if (orderLine.Quantity == 0) _orderLines.Remove(orderLine);
                    break;
            }
        }
    }
}