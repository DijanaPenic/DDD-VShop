using System;
using System.Linq;
using System.Collections.Generic;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;

namespace VShop.Modules.Sales.Domain.Models.Ordering
{
    public class Order : AggregateRoot<EntityId>
    {
        private List<OrderItem> _orderItems;
        
        public Price DeliveryCost { get; private set; }
        public Price ProductsCostWithoutDiscount  => new(_orderItems.Sum(sci => sci.TotalAmount));
        public Price TotalDiscount { get; private set; }
        public Price ProductsCostWithDiscount => ProductsCostWithoutDiscount - TotalDiscount;
        public Price FinalAmount => ProductsCostWithDiscount + DeliveryCost;
        public IReadOnlyCollection<OrderItem> Items => _orderItems;
        public OrderStatus Status { get; private set; }
        public OrderCustomer Customer { get; private set; }

        public static Order Create
        (
            EntityId orderId,
            Price deliveryCost,
            Price totalDiscount,
            EntityId customerId,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            Address deliveryAddress,
            Guid messageId,
            Guid correlationId
        )
        {
            Order order = new()
            {
                CorrelationId = correlationId,
                MessageId = messageId,
            };
            
            order.RaiseEvent
            (
                new OrderPlacedDomainEvent
                {
                    OrderId = orderId,
                    DeliveryCost = deliveryCost,
                    TotalDiscount = totalDiscount,
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

        public Option<ApplicationError> AddOrderItem
        (
            EntityId productId,
            ProductQuantity quantity,
            Price unitPrice
        )
        {
            RaiseEvent
            (
                new OrderItemAddedDomainEvent
                {
                    OrderId = Id,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                }
            );

            return Option<ApplicationError>.None;
        }

        public Option<ApplicationError> SetCancelledStatus()
        {
            if(Status != OrderStatus.Processing)
                return ValidationError.Create($"Changing status to 'Cancelled' is not allowed. Order Status: '{Status}'.");
            
            RaiseEvent(new OrderStatusSetToCancelledDomainEvent{ OrderId = Id });
            
            return Option<ApplicationError>.None;
        }
        
        public Option<ApplicationError> SetShippedStatus()
        {
            if(Status != OrderStatus.Processing)
                return ValidationError.Create($"Changing status to 'Shipped' is not allowed. Order Status: '{Status}'.");
            
            RaiseEvent(new OrderStatusSetToShippedDomainEvent{ OrderId = Id });
            
            return Option<ApplicationError>.None;
        }

        protected override void ApplyEvent(IDomainEvent @event)
        {
            switch (@event)
            {
                case OrderPlacedDomainEvent e:
                    Id = new EntityId(e.OrderId);
                    TotalDiscount = new Price(e.TotalDiscount);
                    DeliveryCost = new Price(e.DeliveryCost);

                    // one-to-one relationship
                    OrderCustomer orderCustomer = new(RaiseEvent);
                    ApplyToEntity(orderCustomer, e);
                    Customer = orderCustomer;
                    
                    Status = OrderStatus.Processing;
                    
                    // one-to-many relationship
                    _orderItems = new List<OrderItem>();
                    break;
                case OrderItemAddedDomainEvent e:
                    OrderItem orderItem = new(RaiseEvent);
                    ApplyToEntity(orderItem, e);
                    _orderItems.Add(orderItem);
                    break;
                case OrderStatusSetToCancelledDomainEvent _:
                    Status = OrderStatus.Cancelled;
                    break;
                case OrderStatusSetToShippedDomainEvent _:
                    Status = OrderStatus.Shipped;
                    break;
            }
        }
    }
}