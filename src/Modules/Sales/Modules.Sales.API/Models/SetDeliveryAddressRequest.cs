using System.ComponentModel.DataAnnotations;

namespace VShop.Modules.Sales.API.Models
{
    internal record SetDeliveryAddressRequest
    {
        [Required]
        public string City { get; init; }
        
        [Required]
        public string CountryCode { get; init; }
        
        [Required]
        public string PostalCode { get; init; }
        
        [Required]
        public string StateProvince { get; init; }
        
        [Required]
        public string StreetAddress { get; init; }
    }
}