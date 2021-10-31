using System;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.Modules.Sales.Domain.Events;

namespace VShop.Modules.Sales.Domain.Models.Ordering
{
    public class OrderItem : Entity<EntityId>
    {
        public EntityId OrderId { get; private set; }
        public EntityId ProductId { get; private set; }
        public ProductQuantity Quantity { get; private set; }
        public Price UnitPrice { get; private set; }
        public Price TotalAmount => UnitPrice * Quantity;
        
        public OrderItem(Action<IDomainEvent> applier) : base(applier) { }
        
        protected override void When(IDomainEvent @event)
        {
            switch (@event)
            {
                case OrderItemAddedDomainEvent e:
                    Id = new EntityId(e.OrderItemId);
                    OrderId = new EntityId(e.OrderId);
                    ProductId = new EntityId(e.ProductId);
                    Quantity = new ProductQuantity(e.Quantity);
                    UnitPrice = new Price(e.UnitPrice);
                    break;
            }
        }
    }
}