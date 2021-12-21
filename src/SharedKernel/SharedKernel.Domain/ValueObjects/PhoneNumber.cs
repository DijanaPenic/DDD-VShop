using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using VShop.SharedKernel.Infrastructure;

[assembly: InternalsVisibleTo("VShop.Modules.Sales.Domain")]
namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class PhoneNumber : ValueObject
    {
        public string Value { get; }

        [JsonConstructor]
        internal PhoneNumber(string value) => Value = value;

        public static Result<PhoneNumber> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.ValidationError("The phone number is required.");
            // TODO - implement other email checks
            
            return new PhoneNumber(value);
        }
        
        public static implicit operator string(PhoneNumber self) => self.Value;

        public override string ToString() => Value;
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}