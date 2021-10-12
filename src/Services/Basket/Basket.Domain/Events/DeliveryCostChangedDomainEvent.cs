using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Events
{
    public class DeliveryCostChangedDomainEvent : IDomainEvent
    {
        public decimal DeliveryCost { get; set; }
    }
}