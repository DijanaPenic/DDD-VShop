using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.API.ValidationAttributes;

namespace VShop.Modules.Sales.API.Models
{
    public record SetContactInformationRequest
    {
        [Required]
        public string FirstName { get; init; }
        
        public string MiddleName { get; init; }
        
        [Required]
        public string LastName { get; init; }
        
        [Required, Email]
        public string EmailAddress { get; init; }
        
        [Required, PhoneNumber]
        public string PhoneNumber { get; init; }
        
        [Required]
        public Gender Gender { get; init; }
    }
}