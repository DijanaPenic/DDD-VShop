using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Types;

[assembly: InternalsVisibleTo("VShop.Modules.Sales.Domain")]
namespace VShop.SharedKernel.Domain.ValueObjects
{
    public class EntityId : ValueObject
    {
        public Guid Value { get; }

        [JsonConstructor]
        internal EntityId(Guid value) => Value = value;
        
        public static Result<EntityId> Create(Guid value)
        {
            if (SequentialGuid.IsNullOrEmpty(value))
                return Result.ValidationError("The entity Id cannot be empty.");
        
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