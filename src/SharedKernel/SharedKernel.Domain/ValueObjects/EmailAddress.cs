using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using VShop.SharedKernel.Infrastructure;

[assembly: InternalsVisibleTo("VShop.Modules.Sales.Domain")]
namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class EmailAddress : ValueObject
    {
        public string Value { get; }

        [JsonConstructor]
        internal EmailAddress(string value) => Value = value;

        public static Result<EmailAddress> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.ValidationError("The email address is required.");
            // TODO - implement other email checks
            
            return new EmailAddress(value);
        }
        
        public static implicit operator string(EmailAddress self) => self.Value;

        public override string ToString() => Value;
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}