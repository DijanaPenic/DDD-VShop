using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record OrderLineOutOfStockRemoved : DomainEvent
    {
        public Guid OrderId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}