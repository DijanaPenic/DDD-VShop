using System;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.Enums;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartContactInformationSetDomainEvent : MessageContext, IDomainEvent
    {
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