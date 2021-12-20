using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Application.Extensions;

namespace VShop.Modules.Sales.API.Models
{
    public record SetContactInformationRequest
    {
        public string FirstName { get; init; }
        public string MiddleName { get; init; }
        public string LastName { get; init; }
        public string EmailAddress { get; init; }
        public string PhoneNumber { get; init; }
        public GenderType Gender { get; init; }
    }
}