using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using VShop.SharedKernel.Infrastructure;

[assembly: InternalsVisibleTo("VShop.Modules.Sales.Domain")]
namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class ProductQuantity : ValueObject
    {
        public int Value { get; }
        
        [JsonConstructor]
        internal ProductQuantity(int value) => Value = value;

        public static Result<ProductQuantity> Create(int value)
        {
            if (value <= 0)
                return Result.ValidationError("Quantity value must be larger than 0.");

            return new ProductQuantity(value);
        }

        public static implicit operator int(ProductQuantity self) => self.Value;
        
        public static ProductQuantity operator +(ProductQuantity self, ProductQuantity increment)
            => new(self.Value + increment.Value);
        
        public static ProductQuantity operator -(ProductQuantity self, ProductQuantity decrement)
            => new(self.Value - decrement.Value);

        public override string ToString() => Value.ToString();
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}