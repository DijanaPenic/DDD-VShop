using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("VShop.Services.Sales.Domain")]
namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public string City { get; }

        public string CountryCode { get; }

        public string PostalCode { get; }

        public string StateProvince { get; }

        public string StreetAddress { get; }

        internal Address
        (
            string city,
            string countryCode,
            string postalCode,
            string stateProvince,
            string streetAddress
        )
        {
            City = city;
            CountryCode = countryCode;
            PostalCode = postalCode;
            StateProvince = stateProvince;
            StreetAddress = streetAddress;
        }

        public static Address Create
        (
            string city,
            string countryCode,
            string postalCode,
            string stateProvince,
            string streetAddress
        )
        {
            // TODO - implement validation
            
            return new Address(city, countryCode, postalCode, stateProvince, streetAddress);
        }
        
        public override string ToString()
        {
            return "PostalAddress [streetAddress=" + StreetAddress
                                                   + ", city=" + City + ", stateProvince=" + StateProvince
                                                   + ", postalCode=" + PostalCode
                                                   + ", countryCode=" + CountryCode + "]";
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return City;
            yield return CountryCode;
            yield return PostalCode;
            yield return StateProvince;
            yield return StreetAddress;
        }
    }
}