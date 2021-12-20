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
}