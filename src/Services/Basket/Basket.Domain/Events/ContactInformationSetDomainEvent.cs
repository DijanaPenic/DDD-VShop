using System;

using VShop.SharedKernel.Infrastructure.Domain;
using VShop.SharedKernel.Infrastructure.Domain.Enums;

namespace VShop.Services.Basket.Domain.Events
{
    public record ContactInformationSetDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public GenderType Gender { get; set; }
    }
}