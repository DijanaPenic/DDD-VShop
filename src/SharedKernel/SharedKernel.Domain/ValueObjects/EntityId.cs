using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

[assembly: InternalsVisibleTo("VShop.Services.ShoppingCarts.Domain")]
namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class EntityId : ValueObject
    {
        public Guid Value { get; }
        
        internal EntityId(Guid value) => Value = value;
        
        public static EntityId Create(Guid value)
        {
            if (value == default) // TODO - check sequential Guid
                throw new ValidationException("The entity Id cannot be empty."); // TODO - use custom assert class

            return new EntityId(value);
        }

        public static implicit operator Guid(EntityId self) => self.Value;

        public override string ToString() => Value.ToString();
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}