using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.EventSourcing.Aggregates;

[assembly: InternalsVisibleTo("VShop.Modules.Sales.Domain")]
namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class EmailAddress : ValueObject
    {
        public string Value { get; }

        internal EmailAddress(string value) => Value = value;

        public static EmailAddress Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException("The email address is required.");
            // TODO - implement other email checks
            
            return new EmailAddress(value);
        }
        
        public static implicit operator string(EmailAddress self) 
            => self.Value;

        public override string ToString() => Value;
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}