using System;
using OneOf.Types;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Application.Commands;

namespace VShop.Services.ShoppingCarts.API.Application.Commands
{
    public record SetContactInformationCommand : ICommand<Success>
    {
        public Guid ShoppingCartId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public GenderType Gender { get; set; }
    }
}