using System;

using VShop.SharedKernel.Domain;
using VShop.SharedKernel.Domain.Enums;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ContactInformationSetDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public string FirstName { get; init; }
        public string MiddleName { get; init; }
        public string LastName { get; init; }
        public string EmailAddress { get; init; }
        public string PhoneNumber { get; init; }
        public GenderType Gender { get; init; }
    }
}