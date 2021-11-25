using System;

using VShop.Modules.Sales.Domain.Events;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Aggregates;

namespace VShop.Modules.Sales.Domain.Models.Ordering
{
    public class OrderLine : Entity<EntityId>
    {
        public EntityId OrderId { get; private set; }
        public ProductQuantity Quantity { get; private set; }
        public Price UnitPrice { get; private set; }
        public Price TotalAmount => UnitPrice * Quantity;
        
        public OrderLine(Action<IDomainEvent> applier) : base(applier) { }
        
        protected override void ApplyEvent(IDomainEvent @event)
        {
            switch (@event)
            {
                case OrderLineAddedDomainEvent e:
                    Id = new EntityId(e.ProductId);
                    OrderId = new EntityId(e.OrderId);
                    Quantity = new ProductQuantity(e.Quantity);
                    UnitPrice = new Price(e.UnitPrice);
                    break;
            }
        }
    }
}