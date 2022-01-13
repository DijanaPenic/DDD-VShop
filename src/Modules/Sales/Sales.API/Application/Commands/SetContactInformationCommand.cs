using System;

using VShop.SharedKernel.Domain.Enums;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class SetContactInformationCommand
    {
        public SetContactInformationCommand
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