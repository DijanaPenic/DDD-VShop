using System;

using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    public partial class SetContactInformationCommand : MessageContext, ICommand
    {
        public SetContactInformationCommand
        (
            Guid shoppingCartId,
            string firstName,
            string middleName,
            string lastName,
            string emailAddress,
            string phoneNumber,
            Gender gender,
            MessageMetadata metadata
        )
        {
            ShoppingCartId = shoppingCartId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            EmailAddress = emailAddress;
            PhoneNumber = phoneNumber;
            Gender = gender;
            Metadata = metadata;
        }
    }
}