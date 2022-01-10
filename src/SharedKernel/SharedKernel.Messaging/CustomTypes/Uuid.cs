using System;

namespace VShop.SharedKernel.Messaging.CustomTypes
{
    public partial class Uuid
    {
        public static implicit operator Guid(Uuid value) => Guid.Parse(value.Value);
        public static implicit operator Uuid(Guid value) => new() { Value = value.ToString() };
    }
}