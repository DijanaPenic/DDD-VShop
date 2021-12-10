using System.Linq;
using System.Collections.Generic;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;

namespace VShop.Modules.Sales.Domain.Models.Ordering
{
    public class Order : AggregateRoot<EntityId>
    {
        private readonly List<OrderLine> _orderLines = new();
        
        public Price DeliveryCost { get; private set; }
        public Price ProductsCostWithoutDiscount => new(_orderLines.Sum(ol => ol.TotalAmount));
        public Price TotalDiscount { get; private set; }
        public Price ProductsCostWithDiscount => ProductsCostWithoutDiscount - TotalDiscount;
        public Price FinalAmount => ProductsCostWithDiscount + DeliveryCost;
        public IReadOnlyCollection<OrderLine> OrderLines => _orderLines;
        public OrderStatus Status { get; private set; }
        public OrderCustomer Customer { get; private set; }

        public Result Create
        (
            EntityId orderId,
            Price deliveryCost,
            Price totalDiscount, // TODO - missing total payment amount
            EntityId customerId,
            FullName fullName,
            EmailAddress emailAddress,
            PhoneNumber phoneNumber,
            Address deliveryAddress
        )
        {
            RaiseEvent
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

            return Result.Success;
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

        public Result SetCancelledStatus()
        {
            if(Status is not OrderStatus.Processing)
                return Result.ValidationError($"Changing status to '{OrderStatus.Cancelled}' is not allowed. Order Status: '{Status}'.");
            
            RaiseEvent(new OrderStatusSetToCancelledDomainEvent{ OrderId = Id });
            
            return Result.Success;
        }
        
        public Result SetShippedStatus()
        {
            if(Status is not OrderStatus.Processing)
                return Result.ValidationError($"Changing status to '{OrderStatus.Shipped}' is not allowed. Order Status: '{Status}'.");
            
            RaiseEvent(new OrderStatusSetToShippedDomainEvent{ OrderId = Id });
            
            return Result.Success;
        }

        protected override void ApplyEvent(IDomainEvent @event)
        {
            switch (@event)
            {
                case OrderPlacedDomainEvent e:
                    Id = new EntityId(e.OrderId);
                    TotalDiscount = new Price(e.TotalDiscount);
                    DeliveryCost = new Price(e.DeliveryCost);
                    Status = OrderStatus.Processing;

                    // one-to-one relationship
                    OrderCustomer orderCustomer = new(RaiseEvent);
                    ApplyToEntity(orderCustomer, e);
                    Customer = orderCustomer;
                    break;
                case OrderLineAddedDomainEvent e:
                    OrderLine orderLine = new(RaiseEvent);
                    ApplyToEntity(orderLine, e);
                    _orderLines.Add(orderLine);
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