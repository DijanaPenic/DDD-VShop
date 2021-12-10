using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

[assembly: InternalsVisibleTo("VShop.Modules.Sales.Domain")]
namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class Discount : ValueObject
    {
        public int Value { get; }
        
        internal Discount(int value) => Value = value;

        public static Discount Create(int value)
        {
            if (value is < 0 or > 100)
                throw new ValidationException("Discount must be larger than 0 and less than 100.");

            return new Discount(value);
        }

        public static implicit operator int(Discount self) => self.Value;

        public override string ToString() => Value.ToString();
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}