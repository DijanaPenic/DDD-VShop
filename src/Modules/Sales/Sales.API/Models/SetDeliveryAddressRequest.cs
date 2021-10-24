using FluentValidation;

namespace VShop.Modules.Sales.API.Models
{
    public record SetDeliveryAddressRequest
    {
        public string City { get; init; }
        public string CountryCode { get; init; }
        public string PostalCode { get; init; }
        public string StateProvince { get; init; }
        public string StreetAddress { get; init; }
    }
    
    public class SetDeliveryAddressRequestValidator : AbstractValidator<SetDeliveryAddressRequest> {
        public SetDeliveryAddressRequestValidator()
        {
            RuleFor(da => da.City).NotEmpty();
            RuleFor(da => da.CountryCode).NotEmpty();
            RuleFor(da => da.PostalCode).NotEmpty();
            RuleFor(da => da.StateProvince).NotEmpty();
            RuleFor(da => da.StreetAddress).NotEmpty();
        }
    }
}