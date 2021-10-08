using System;
using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Domain
{
    public class EntityId : ValueObject
    {
        public EntityId(Guid value)
        {
            if (value == default) // TODO - check sequential Guid
                throw new ArgumentNullException(nameof(value), "The Id cannot be empty.");

            Value = value;
        }

        public Guid Value { get; }
        
        public static implicit operator Guid(EntityId self) => self.Value;

        public override string ToString() => Value.ToString();
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}