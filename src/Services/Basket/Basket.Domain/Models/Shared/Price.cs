﻿using System;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.Basket.Domain.Models.Shared
{
    public class Price : ValueObject
    {
        public decimal Value { get; }
        
        internal Price(decimal value) => Value = value;

        public static Price Create(decimal value)
        {
            if (value < 0)
                throw new ArgumentNullException(nameof(value), "Price must be larger than 0 or equal to 0.");

            return new Price(value);
        }

        public static implicit operator decimal(Price self) 
            => self.Value;
        
        public static Price operator +(Price self, Price increment)
            => new(self.Value + increment.Value);
        
        public static Price operator -(Price self, Price decrement)
            => new(self.Value - decrement.Value);

        public static Price operator *(Price self, decimal multiplier)
            => new(self.Value * multiplier);
        
        public void Deconstruct(out decimal value)
        {
            value = Value;
        }

        public override string ToString()
            => Value.ToString();
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}