using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Events
{
    public class DeliveryCostChangedDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; set; }
        public decimal DeliveryCost { get; set; }
    }
}