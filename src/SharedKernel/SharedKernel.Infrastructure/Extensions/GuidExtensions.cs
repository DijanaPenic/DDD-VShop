using System;

using VShop.SharedKernel.Infrastructure.Types;

namespace VShop.SharedKernel.Infrastructure.Extensions
{
    public static class GuidExtensions
    {
        public static Uuid ToUuid(this Guid value) => new() { Value = value.ToString() };
    }
}