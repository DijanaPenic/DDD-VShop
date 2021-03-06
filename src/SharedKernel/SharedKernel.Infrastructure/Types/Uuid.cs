using System;

namespace VShop.SharedKernel.Infrastructure.Types
{
    public partial class Uuid
    {
        public static implicit operator Guid(Uuid value) => Guid.Parse(value.Value);
        public static implicit operator Uuid(Guid value) => new() { Value = value.ToString() };
        public static Uuid NewSequentialUuId() => SequentialGuid.Create();
    }
}