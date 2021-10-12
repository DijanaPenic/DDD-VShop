using System;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Models.BasketAggregate
{
    public class Quantity : ValueObject
    {
        public int Value { get; }
        
        internal Quantity(int value) => Value = value;

        public static Quantity Create(int value)
        {
            if (value > 0)
                throw new ArgumentNullException(nameof(value), "Quantity value must be larger than 0.");

            return new Quantity(value);
        }

        public static implicit operator int(Quantity self) 
            => self.Value;
        
        public static Quantity operator +(Quantity self, Quantity increment)
            => new(self.Value + increment.Value);
        
        public static Quantity operator -(Quantity self, Quantity decrement)
            => new(self.Value - decrement.Value);

        public override string ToString()
            => Value.ToString();
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}