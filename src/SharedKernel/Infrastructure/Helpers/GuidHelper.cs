using System;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace VShop.SharedKernel.Infrastructure.Helpers
{
    public static class GuidHelper
    {
        private static SequentialGuidValueGenerator _sequentialGuidGenerator;

        private static SequentialGuidValueGenerator SequentialGuidGenerator
        {
            get
            {
                _sequentialGuidGenerator ??= new SequentialGuidValueGenerator();

                return _sequentialGuidGenerator;
            }
        }

        public static Guid NewSequentialGuid() => SequentialGuidGenerator.Next(null!);

        public static bool IsNullOrEmpty(object value)
        {
            return ((value as Guid?).GetValueOrDefault() == Guid.Empty);
        }
    }
}