using System;
using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Domain
{
    public class EntityId : ValueObject
    {
        // TODO - check if I can make constructor private and add a method for Guid value.
        public EntityId(Guid value)
        {
            if (value == default) // TODO - check sequential Guid
                throw new ArgumentNullException(nameof(value), "The entity Id cannot be empty."); // TODO - use custom assert class

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