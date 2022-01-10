using System;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartContactInformationSetDomainEvent //: DomainEvent
    {
        public Guid ShoppingCartId { get; }
        public string FirstName { get; }
        public string MiddleName { get; }
        public string LastName { get; }
        public string EmailAddress { get; }
        public string PhoneNumber { get; }
        public GenderType Gender { get; }

        public ShoppingCartContactInformationSetDomainEvent
        (
            Guid shoppingCartId,
            string firstName,
            string middleName,
            string lastName,
            string emailAddress,
            string phoneNumber,
            GenderType gender
        )
        {
            ShoppingCartId = shoppingCartId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            EmailAddress = emailAddress;
            PhoneNumber = phoneNumber;
            Gender = gender;
        }
    }
}