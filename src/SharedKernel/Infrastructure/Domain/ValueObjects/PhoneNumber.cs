using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("VShop.Services.Basket.Domain")]
namespace VShop.SharedKernel.Infrastructure.Domain.ValueObjects
{
    public class PhoneNumber : ValueObject
    {
        public string Value { get; }

        internal PhoneNumber(string value) => Value = value;

        public static PhoneNumber Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("The phone number is required.");
            // TODO - implement other email checks
            
            return new PhoneNumber(value);
        }
        
        public static implicit operator string(PhoneNumber self) 
            => self.Value;

        public override string ToString() => Value;
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}