using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartCreatedDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; }
        public Guid CustomerId { get; }
        public int CustomerDiscount { get; }

        public ShoppingCartCreatedDomainEvent
        (
            Guid shoppingCartId,
            Guid customerId,
            int customerDiscount
        )
        {
            ShoppingCartId = shoppingCartId;
            CustomerId = customerId;
            CustomerDiscount = customerDiscount;
        }
    }
}