using System;

using VShop.Modules.Sales.Domain.Events;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Models.Ordering
{
    internal class OrderLine : Entity<EntityId>
    {
        public EntityId OrderId { get; private set; }
        public ProductQuantity Quantity { get; private set; }
        public Price UnitPrice { get; private set; }
        public Price TotalAmount => UnitPrice * Quantity;
        
        public OrderLine(Action<IDomainEvent> applier) : base(applier) { }
        
        public Result RemoveOutOfStockItems(ProductQuantity value)
        {
            if (Quantity - value < 0)
                return Result.ValidationError($"Cannot decrease quantity by {value}.");
            
            RaiseEvent
            (
                new OrderLineOutOfStockRemovedDomainEvent
                (
                    OrderId,
                    Id,
                    value
                )
            );

            return Result.Success;
        }
        
        protected override void ApplyEvent(IDomainEvent @event)
        {
            switch (@event)
            {
                case OrderLineAddedDomainEvent e:
                    Id = new EntityId(e.ProductId);
                    OrderId = new EntityId(e.OrderId);
                    Quantity = new ProductQuantity(e.Quantity);
                    UnitPrice = new Price(e.UnitPrice.DecimalValue);
                    break;
                case OrderLineOutOfStockRemovedDomainEvent e:
                    Quantity -= new ProductQuantity(e.Quantity);
                    break;
            }
        }
    }
}