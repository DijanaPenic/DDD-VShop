using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

[assembly: InternalsVisibleTo("VShop.Modules.Sales.Domain")]
namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class Price : ValueObject
    {
        public decimal Value { get; }
        
        internal Price(decimal value) => Value = value;

        public static Price Create(decimal value)
        {
            if (value < 0)
                throw new ValidationException("Price must be larger than 0 or equal to 0.");

            return new Price(value);
        }

        public static implicit operator decimal(Price self) => self.Value;
        
        public static Price operator ++(Price self) => new(self.Value + 1);
        
        public static Price operator +(Price self, Price increment) => new(self.Value + increment.Value);
        
        public static Price operator -(Price self, Price decrement) => new(self.Value - decrement.Value);

        public static Price operator *(Price self, decimal multiplier) => new(self.Value * multiplier);
        
        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}