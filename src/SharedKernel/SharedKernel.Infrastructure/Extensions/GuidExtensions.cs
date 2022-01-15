﻿using System;

using VShop.SharedKernel.Infrastructure.Types;

namespace VShop.SharedKernel.Infrastructure.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsNullOrEmpty(this Guid value) => SequentialGuid.IsNullOrEmpty(value);
    }
}