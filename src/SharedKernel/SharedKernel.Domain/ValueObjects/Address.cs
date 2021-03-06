using Newtonsoft.Json;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public string City { get; }
        public string CountryCode { get; }
        public string PostalCode { get; }
        public string StateProvince { get; }
        public string StreetAddress { get; }

        [JsonConstructor]
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

        public static Result<Address> Create
        (
            string city,
            string countryCode,
            string postalCode,
            string stateProvince,
            string streetAddress
        )
        {
            // No validation, for now.
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