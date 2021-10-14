using System;

using VShop.SharedKernel.Infrastructure.Helpers;

namespace VShop.SharedKernel.Infrastructure.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsNullOrEmpty(this Guid value) => GuidHelper.IsNullOrEmpty(value);
    }
}