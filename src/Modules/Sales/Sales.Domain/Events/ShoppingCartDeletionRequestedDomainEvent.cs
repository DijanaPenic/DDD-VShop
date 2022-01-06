using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartDeletionRequestedDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; }
        
        public ShoppingCartDeletionRequestedDomainEvent(Guid shoppingCartId)
        {
            ShoppingCartId = shoppingCartId;
        }
    }
}