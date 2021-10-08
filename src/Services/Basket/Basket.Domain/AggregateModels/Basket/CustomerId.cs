using System;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure;

namespace VShop.Services.Basket.Domain.AggregateModels.Basket
{
    public class CustomerId : ValueObject
    {
        internal CustomerId(Guid value)
        {
            Value = value;
        }

        public static CustomerId FromGuid(Guid value)
        {
            if (value == default) // TODO - check sequential Guid
                throw new ArgumentNullException(nameof(value), "The Id cannot be empty.");

            return new CustomerId(value);
        }

        public Guid Value { get; }
        
        public static implicit operator Guid(CustomerId self) => self.Value;

        public override string ToString() => Value.ToString();
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}