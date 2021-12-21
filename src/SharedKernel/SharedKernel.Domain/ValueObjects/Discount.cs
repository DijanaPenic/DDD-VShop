using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using VShop.SharedKernel.Infrastructure;

[assembly: InternalsVisibleTo("VShop.Modules.Sales.Domain")]
namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class Discount : ValueObject
    {
        public int Value { get; }
        
        [JsonConstructor]
        internal Discount(int value) => Value = value;

        public static Result<Discount> Create(int value)
        {
            if (value is < 0 or > 100)
                return Result.ValidationError("Discount must be larger than 0 and less than 100.");

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