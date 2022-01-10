using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartDeletedDomainEvent// : DomainEvent
    {
        public Guid ShoppingCartId { get; }
        
        public ShoppingCartDeletedDomainEvent(Guid shoppingCartId)
        {
            ShoppingCartId = shoppingCartId;
        }
    }
}