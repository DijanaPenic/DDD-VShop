using System;

using VShop.SharedKernel.Infrastructure.Domain;
using VShop.SharedKernel.Infrastructure.Domain.Enums;

namespace VShop.Services.Basket.Domain.Events
{
    public record ContactInformationSetDomainEvent : IDomainEvent
    {
        public Guid BasketCustomerId { get; init; }
        public string FirstName { get; init; }
        public string MiddleName { get; init; }
        public string LastName { get; init; }
        public string EmailAddress { get; init; }
        public string PhoneNumber { get; init; }
        public GenderType Gender { get; init; }
    }
}